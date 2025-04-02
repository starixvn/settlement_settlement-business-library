using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Services;
public record PrepareTicketParameters
{
    public Ticket Ticket { get; init; }
    public byte StatusId { get; init; }
    public string StatusName { get; init; }
    public bool IsApplyMaxPayout { get; init; }
    public decimal? MaxWinLost { get; init; }
    public decimal CommWinRate { get; init; }
    public decimal CommLostRate { get; init; }
    public byte? StatusWinLost { get; init; }
}

public interface PrepareTicketService
{
    PrepareTicket CreateValue(Ticket ticket, List<WinLimit> maxWinLimit);

    PrepareTicket RecoverValue(Ticket ticket, List<WinLimit> maxWinLimit);

    Commission CreateCommission(Ticket ticket, PrepareTicket infos);

    Commission RecoverCommission(Ticket ticket, PrepareTicket infos);

    Winlost CreateWinLost(Ticket ticket, PrepareTicket infos);

    Winlost RecoverWinLost(Ticket ticket, PrepareTicket infos);

    bool IsTicketUpdatable(Ticket ticket);
}

public class PrepareTicketServiceImp(
    CommissionService commissionService,
    WinLostByModdsService winLostByModdsService,
    WinLostService winLostService) : PrepareTicketService
{
    private const decimal CommissionHalfRate = 0.5m;
    private const decimal DefaultOdds = 1m;
    private const string CommissionStatusOne = "1";

    public PrepareTicket CreateValue(Ticket ticket, List<WinLimit> maxWinLimit)
    {
        var (statusId, statusName) = Status(ticket.Modds);
        var (isApplyMaxPayout, maxWinLost) = winLostService.GetWinLost(ticket, maxWinLimit, false);
        var (commWinRate, commLostRate) = GetCommissionRates(ticket.ComStatus);
        var statusWinLost = StatusWinLost(ticket.Modds);

        var parameters = new PrepareTicketParameters
        {
            Ticket = ticket,
            StatusId = statusId,
            StatusName = statusName,
            IsApplyMaxPayout = isApplyMaxPayout,
            MaxWinLost = maxWinLost,
            CommWinRate = commWinRate,
            CommLostRate = commLostRate,
            StatusWinLost = statusWinLost
        };

        return CreatePrepareTicket(parameters);
    }

    public PrepareTicket RecoverValue(Ticket ticket, List<WinLimit> maxWinLimit)
    {
        var (statusId, statusName) = RecoverStatus(ticket.StatusWinLost);
        var (isApplyMaxPayout, maxWinLost) = winLostService.GetWinLost(ticket, maxWinLimit, true);
        var (commWinRate, commLostRate) = GetCommissionRates(ticket.ComStatus);

        var parameters = new PrepareTicketParameters
        {
            Ticket = ticket,
            StatusId = statusId,
            StatusName = statusName,
            IsApplyMaxPayout = isApplyMaxPayout,
            MaxWinLost = maxWinLost,
            CommWinRate = commWinRate,
            CommLostRate = commLostRate,
            StatusWinLost = ticket.StatusWinLost
        };

        return CreatePrepareTicket(parameters);
    }

    private static PrepareTicket CreatePrepareTicket(PrepareTicketParameters parameters)
    {
        return new PrepareTicket
        {
            CommWinRate = parameters.CommWinRate,
            CommLostRate = parameters.CommLostRate,
            CommHalfWLRate = CommissionHalfRate,
            Odds = CalculateOdds(parameters.Ticket),
            StatusId = parameters.StatusId,
            Status = parameters.StatusName,
            StatusWinLost = parameters.StatusWinLost,
            IsApplyMaxPayout = parameters.IsApplyMaxPayout,
            MaxWinLost = parameters.MaxWinLost,
            ExtraInfo = GenerateExtraInfo(parameters.IsApplyMaxPayout, parameters.Ticket.ExtraInfo, parameters.Ticket.ActualPayout)
        };
    }

    private static decimal CalculateOdds(Ticket ticket)
    {
        if (ticket.Odds < 0)
        {
            return DefaultOdds;
        }

        return ticket.OddsType == 1 ? GetOddsByTicket(ticket) : ticket.Odds;
    }

    private static decimal GetOddsByTicket(Ticket ticket)
    {
        if (ticket.BetType == BetTypes.FirstHalfHomeTeamOddEven)
        {
            return ticket.Modds - 1;
        }

        if (IsBackLayBetType(ticket))
        {
            return DefaultOdds;
        }

        return ticket.Odds - 1;
    }

    private static bool IsBackLayBetType(Ticket ticket) =>
        ticket.Modds > Modds.ModdsQuarter &&
        BetTypes.BackLayBetType.Contains(ticket.BetType) &&
        ticket.BetTeam.EndsWith(BetTeam.L);

    private static (decimal WinRate, decimal LostRate) GetCommissionRates(string commStatus) =>
        commStatus == CommissionStatusOne ? (0, 2) : (1, 1);

    private static (byte, string) Status(decimal modds)
    {
        return modds switch
        {
            > 0 => (StatusId.Won, StatusName.Won),
            0 => (StatusId.Draw, StatusName.Draw),
            _ => (StatusId.Lose, StatusName.Lose)
        };
    }

    private static (byte, string) RecoverStatus(byte? statusWinLost)
    {
        return statusWinLost switch
        {
            StatusWinlost.Win or StatusWinlost.HalfWin => (StatusId.Won, StatusName.Won),
            StatusWinlost.Loss or StatusWinlost.HalfLoss => (StatusId.Lose, StatusName.Lose),
            StatusWinlost.Refund => (StatusId.Refund, StatusName.Refund),
            _ => (StatusId.Draw, StatusName.Draw)
        };
    }

    private static byte StatusWinLost(decimal mOdds)
    {
        return mOdds > 0 ? StatusWinlost.Win : StatusWinlost.Loss;
    }

    private static string GenerateExtraInfo(bool isApplyMaxPayout, string extraValue, decimal? actualPayout)
    {
        if (!isApplyMaxPayout || actualPayout is null)
        {
            return extraValue;
        }

        var newExtraInfo = $"<l255 apo=\"{Math.Round(actualPayout.Value, 2)}\" />";
        return string.IsNullOrWhiteSpace(extraValue) ? newExtraInfo : $"{extraValue}{newExtraInfo}";
    }

    public Commission CreateCommission(Ticket ticket, PrepareTicket infos)
    {
        return ticket.Modds switch
        {
            < Modds.NegativeModdsQuarter => commissionService.ModdsLowerThanNegativeQuarter(ticket, infos),
            Modds.NegativeModdsQuarter => commissionService.ModdsEqualNegativeQuarter(ticket, infos),
            Modds.ModdsQuarter => commissionService.ModdsEqualQuarter(ticket, infos),
            > Modds.ModdsQuarter => commissionService.ModdsHigherThanQuarter(ticket, infos),
            _ => new Commission()
        };
    }

    public Commission RecoverCommission(Ticket ticket, PrepareTicket infos)
    {
        return ticket.StatusWinLost switch
        {
            StatusWinlost.Loss => commissionService.ModdsLowerThanNegativeQuarter(ticket, infos),
            StatusWinlost.HalfLoss => commissionService.ModdsEqualNegativeQuarter(ticket, infos),
            StatusWinlost.HalfWin => commissionService.ModdsEqualQuarter(ticket, infos),
            StatusWinlost.Win => commissionService.ModdsHigherThanQuarter(ticket, infos),
            _ => new Commission()
        };
    }

    public Winlost CreateWinLost(Ticket ticket, PrepareTicket infos)
    {
        if (infos is { MaxWinLost: not null, IsApplyMaxPayout: true })
        {
            return winLostByModdsService.WinLostMaxPayout(ticket, (decimal)infos.MaxWinLost);
        }

        return ticket.Modds switch
        {
            < Modds.NegativeModdsQuarter => winLostByModdsService.WinLostLowerThanNegativeQuarter(ticket),
            Modds.NegativeModdsQuarter => winLostByModdsService.WinLostEqualNegativeQuarter(ticket),
            Modds.ModdsQuarter => winLostByModdsService.WinLostEqualQuarter(ticket, infos.Odds),
            > Modds.ModdsQuarter => winLostByModdsService.WinLostGreaterThanQuarter(ticket, infos.Odds),
            _ => new Winlost { WinLost = ticket.ActualStake }
        };
    }

    public Winlost RecoverWinLost(Ticket ticket, PrepareTicket infos)
    {
        if (infos is { MaxWinLost: not null, IsApplyMaxPayout: true })
        {
            return winLostByModdsService.RecoverWinLostMaxPayout(ticket, (decimal)infos.MaxWinLost);
        }

        return ticket.StatusWinLost switch
        {
            StatusWinlost.Loss => winLostByModdsService.WinLostLowerThanNegativeQuarter(ticket),
            StatusWinlost.HalfLoss => winLostByModdsService.WinLostEqualNegativeQuarter(ticket),
            StatusWinlost.HalfWin => winLostByModdsService.WinLostEqualQuarter(ticket, infos.Odds),
            StatusWinlost.Win => winLostByModdsService.WinLostGreaterThanQuarter(ticket, infos.Odds),
            _ => new Winlost { WinLost = ticket.ActualStake }
        };
    }

    public bool IsTicketUpdatable(Ticket ticket) => (ticket.StatusId != StatusId.Void && !ticket.IsMarked);
}