using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Services;

public interface PrepareTicketService
{
    PrepareTicket CreateValue(Ticket ticket, List<WinLimit> maxWinLimit);

    Commission CreateCommission(Ticket ticket, PrepareTicket infos);

    Winlost CreateWinLost(Ticket ticket, PrepareTicket infos);
}

public class PrepareTicketServiceImp(
    CommissionService commissionService,
    WinLostByModdsService winLostByModdsService,
    WinLostService winLostService) : PrepareTicketService
{
    public PrepareTicket CreateValue(Ticket ticket, List<WinLimit> maxWinLimit)
    {
        var (statusId, statusName) = Status(ticket.Modds);
        var (isApplyMaxPayout, maxWinLost) = winLostService.GetWinLost(ticket, maxWinLimit);
        var (commWinRate, commLostRate) = CommRate(ticket.ComStatus);

        return new PrepareTicket
        {
            CommWinRate = commWinRate,
            CommLostRate = commLostRate,
            CommHalfWLRate = 0.5m,
            Odds = Odds(ticket),
            StatusId = statusId,
            Status = statusName,
            StatusWinLost = StatusWinLost(ticket.Modds),
            IsUpdated = ticket.StatusId != StatusId.Void && !ticket.IsMarked,
            IsApplyMaxPayout = isApplyMaxPayout,
            MaxWinLost = maxWinLost,
            ExtraInfo = ExtraInfo(isApplyMaxPayout, ticket.ExtraInfo, ticket.ActualPayout)
        };
    }

    private static decimal Odds(Ticket ticket)
    {
        if (ticket.Odds < 0)
        {
            return 1;
        }

        return ticket.OddsType == 1 ? GetOddsByTicket(ticket) : ticket.Odds;
    }

    private static decimal GetOddsByTicket(Ticket ticket)
    {
        if (ticket.BetType == BetTypes.FirstHalfHomeTeamOddEven)
        {
            return ticket.Modds - 1;
        }

        if (ticket.Modds > Modds.ModdsQuarter && BetTypes.BackLayBetType.Contains(ticket.BetType) &&
            ticket.BetTeam.EndsWith(BetTeam.L))
        {
            return 1;
        }

        return ticket.Odds - 1;
    }

    private static (decimal commWinRate, decimal commLostRate) CommRate(string commStatus)
    {
        return commStatus == "1" ? (0, 2) : (1, 1);
    }

    private static (byte, string) Status(decimal modds)
    {
        return modds switch
        {
            > 0 => (StatusId.Won, StatusName.Won),
            0 => (StatusId.Draw, StatusName.Draw),
            _ => (StatusId.Lose, StatusName.Lose)
        };
    }

    private static byte StatusWinLost(decimal mOdds)
    {
        return mOdds > 0 ? StatusWinlost.Win : StatusWinlost.Lost;
    }

    private static string ExtraInfo(bool isApplyMaxPayout, string extraValue, decimal? actualPayout)
    {
        if (isApplyMaxPayout && actualPayout != null)
        {
            var newExtraInfo = $"<l255 apo=\"{Math.Round((decimal)actualPayout, 2)}\" />";
            return string.IsNullOrWhiteSpace(extraValue) ? newExtraInfo : $"{extraValue}{newExtraInfo}";
        }

        return extraValue;
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
}