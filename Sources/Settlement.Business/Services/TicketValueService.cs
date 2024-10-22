using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface TicketValueService
{
    decimal? GetActualPayout(decimal modds, decimal odds, short oddsType, decimal actualStake, decimal stake);

    void SetValue(Ticket ticket, List<WinLimit> maxWinLimit);
}

public class TicketValueServiceImp(
    PrepareTicketService prepareTicketService) : TicketValueService
{
    public decimal? GetActualPayout(decimal modds, decimal odds, short oddsType, decimal actualStake, decimal stake)
    {
        var newOdds = Odds(odds, oddsType);

        return modds switch
        {
            > Modds.ModdsQuarter => actualStake + stake * newOdds,
            Modds.ModdsQuarter => actualStake + (stake * 0.5m * newOdds),
            _ => null
        };
    }

    private static decimal Odds(decimal value, short oddsType)
    {
        if (value < 0)
        {
            return 1;
        }

        return oddsType == 1 ? value - 1 : value;
    }

    public void SetValue(Ticket ticket, List<WinLimit> maxWinLimit)
    {
        var oldWinLost = ticket.WinLost;
        var oldStatusId = ticket.StatusId;
        var prepareTicket = prepareTicketService.CreateValue(ticket, maxWinLimit);
        ticket.IsApplyMaxPayout = prepareTicket.IsApplyMaxPayout;
        ticket.MaxWinLimit = prepareTicket.MaxWinLost;
        ticket.StatusWinLost = prepareTicket.StatusWinLost;
        ticket.ExtraInfo = prepareTicket.ExtraInfo;

        if (prepareTicket.IsUpdated)
        {
            var newStatusId = prepareTicket.StatusId;
            var winlost = prepareTicketService.CreateWinLost(ticket, prepareTicket);
            ticket.AWinLost = winlost.AWinLost;
            ticket.MWinLost = winlost.MWinLost;
            ticket.SWinLost = winlost.SWinLost;
            ticket.WinLost = winlost.WinLost;

            var commission = prepareTicketService.CreateCommission(ticket, prepareTicket);
            ticket.PlayerComm = commission.PlayerComm;
            ticket.Comm = commission.Comm;
            ticket.AComm = commission.AComm;
            ticket.SComm = commission.SComm;

            ticket.Ruben = Ruben.Default;
            ticket.IsUpdateSettlementTime = IsUpdateSettlementTime(oldWinLost, winlost.WinLost, oldStatusId, newStatusId);
            ticket.StatusId = newStatusId;
            ticket.Status = prepareTicket.Status;
        }
    }

    private static bool IsUpdateSettlementTime(decimal? oldWinLost, decimal? newWinLost, byte oldStatusId, byte newStatusId)
    {
        return oldWinLost != newWinLost || oldStatusId != newStatusId;
    }
}