using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Extensions;
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

        const int finalOdds = 0;
        var winLost = WinLost(ticket, isApplyMaxPayout, maxWinLost);

        return new PrepareTicket
        {
            CommWinRate = commWinRate,
            CommLostRate = commLostRate,
            CommHalfWLRate = 0.5m, // Need to check: CASE WHEN sr.isCN88 = 1 AND sr.isMajorGroup = 1 THEN 1 ELSE 0.5
            Odds = Odds(ticket, finalOdds),
            FinalOdds = finalOdds,
            StatusId = statusId,
            Status = statusName,
            StatusWinLost = StatusWinLost(ticket.Modds),
            IsUpdated = ticket.StatusId != StatusId.Void && !ticket.IsMarked,
            IsApplyMaxPayout = isApplyMaxPayout,
            MaxWinLost = maxWinLost,
            WinLost = winLost,
            ExtraInfo = ExtraInfo(isApplyMaxPayout, ticket.ExtraInfo, ticket.ActualPayout),
            SettlementTime = GetSettlementTime(ticket, winLost, statusId)
        };
    }

    private static DateTime? GetSettlementTime(Ticket ticket, decimal? newWinLost, byte newStatusId)
    {
        if (ticket.WinLost != newWinLost || ticket.StatusId != newStatusId)
        {
            return Convert.ToDateTime(DateTime.Now.ToStandardString());
        }

        return ticket.SettlementTime;
    }

    private static decimal Odds(Ticket ticket, decimal finalOdds)
    {
        if (ticket.Odds < 0)
        {
            return 1;
        }

        return ticket.OddsType == 1 ? GetOddsByTicket(ticket, finalOdds) : ticket.Odds;
    }

    private static decimal GetOddsByTicket(Ticket ticket, decimal finalOdds)
    {
        if (ticket.BetType == BetTypes.FirstHalfHomeTeamOddEven)
        {
            return finalOdds - 1;
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

    private static decimal? WinLost(Ticket ticket, bool isApplyMaxPayout, decimal? maxWinLost)
    {
        if (ticket.WinLost is not null && isApplyMaxPayout)
        {
            return maxWinLost;
        }

        if (ticket.Modds > Modds.ModdsQuarter)
        {
            return ticket.DeadHeat > 0 ? (ticket.Odds / ticket.DeadHeat + 1) * ticket.Stake : ticket.ActualStake + ticket.Stake * ticket.Modds;
        }

        return 0;
    }

    private static byte StatusWinLost(decimal mOdds)
    {
        return mOdds > 0 ? StatusWinlost.Win : StatusWinlost.Lost;
    }

    private static string? ExtraInfo(bool isApplyMaxPayout, string? extraInfo, decimal? actualPayout)
    {
        if (isApplyMaxPayout && actualPayout != null)
        {
            return string.IsNullOrWhiteSpace(extraInfo) ? $"<l255 apo=\"{Math.Round((decimal)actualPayout, 2)}\" />" : $"{extraInfo}<l255 apo=\"{Math.Round((decimal)actualPayout, 2)}\" />";
        }

        return extraInfo;
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
        if (infos is { WinLost: not null, IsApplyMaxPayout: true })
        {
            return winLostByModdsService.WinLostMaxPayout(ticket, infos);
        }

        return ticket.Modds switch
        {
            < Modds.NegativeModdsQuarter => winLostByModdsService.WinLostLowerThanNegativeQuarter(ticket),
            Modds.NegativeModdsQuarter => winLostByModdsService.WinLostEqualNegativeQuarter(ticket),
            Modds.ModdsQuarter => winLostByModdsService.WinLostEqualQuarter(ticket, infos),
            > Modds.ModdsQuarter => winLostByModdsService.WinLostGreaterThanQuarter(ticket, infos),
            _ => new Winlost { WinLost = ticket.ActualStake }
        };
    }
}