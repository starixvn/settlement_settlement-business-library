using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Services;

public interface TicketValueService
{
    decimal? GetActualPayout(decimal modds, decimal odds, short oddsType, decimal actualStake, decimal stake);

    void SetValue(Ticket ticket, List<WinLimit> maxWinLimit);

    void SetValue(Ticket ticket, bool isRecovery);

    void RecoverSetValue(Ticket ticket, List<WinLimit> maxWinLimit);
}

public class TicketValueServiceImp(
    PrepareTicketService prepareTicketService) : TicketValueService
{
    private const decimal DefaultOdds = 1m;
    private const decimal HalfValue = 0.5m;
    private const short OddsTypeOne = 1;

    public decimal? GetActualPayout(decimal modds, decimal odds, short oddsType, decimal actualStake, decimal stake)
    {
        var newOdds = CalculateAdjustedOdds(odds, oddsType);

        return modds switch
        {
            > Modds.ModdsQuarter => actualStake + stake * newOdds,
            Modds.ModdsQuarter => actualStake + (stake * HalfValue * newOdds),
            _ => null
        };
    }

    private static decimal CalculateAdjustedOdds(decimal value, short oddsType)
    {
        if (value < 0)
        {
            return DefaultOdds;
        }

        return oddsType == OddsTypeOne ? value - 1 : value;
    }

    public void SetValue(Ticket ticket, List<WinLimit> maxWinLimit)
    {
        var prepareTicket = prepareTicketService.CreateValue(ticket, maxWinLimit);
        ticket.StatusWinLost = prepareTicket.StatusWinLost;

        UpdateMaxPayoutInfo(ticket, prepareTicket);
        UpdateTicketValues(ticket, prepareTicket, false);
    }

    public void SetValue(Ticket ticket, bool isRecovery)
    {
        if (isRecovery || prepareTicketService.IsTicketUpdatable(ticket))
        {
            ticket.WinLost = ticket.ActualStake;
            ticket.AWinLost = 0;
            ticket.MWinLost = 0;
            ticket.SWinLost = 0;
            ticket.PlayerComm = 0;
            ticket.Comm = 0;
            ticket.AComm = 0;
            ticket.SComm = 0;
            ticket.Ruben = Ruben.Default;
            ticket.StatusId = StatusId.Refund;
            ticket.Status = StatusName.Refund;
        }

        ticket.StatusWinLost = StatusWinlost.Refund;
    }

    public void RecoverSetValue(Ticket ticket, List<WinLimit> maxWinLimit)
    {
        ticket.ActualPayout = CalculateActualPayout(ticket);

        var prepareTicket = prepareTicketService.RecoverValue(ticket, maxWinLimit);

        UpdateMaxPayoutInfo(ticket, prepareTicket);
        UpdateTicketValues(ticket, prepareTicket, true);
    }

    private static decimal? CalculateActualPayout(Ticket ticket)
    {
        var newOdds = CalculateAdjustedOdds(ticket.Odds, ticket.OddsType);

        return ticket.StatusWinLost switch
        {
            StatusWinlost.Win => ticket.ActualStake + ticket.Stake * newOdds,
            StatusWinlost.HalfWin => ticket.ActualStake + (ticket.Stake * HalfValue * newOdds),
            _ => null
        };
    }

    private void UpdateTicketValues(Ticket ticket, PrepareTicket prepareTicket, bool isRecovery)
    {
        if (!prepareTicketService.IsTicketUpdatable(ticket) && !isRecovery)
        {
            return;
        }

        var (oldWinLost, oldStatusId, newStatusId) = GetStatusInfo(ticket, prepareTicket);

        UpdateWinLostValues(ticket, prepareTicket, isRecovery);
        UpdateCommissionValues(ticket, prepareTicket, isRecovery);
        UpdateStatusInfo(ticket, prepareTicket, oldWinLost, oldStatusId, newStatusId);
    }

    private static (decimal? OldWinLost, byte OldStatusId, byte NewStatusId) GetStatusInfo(Ticket ticket, PrepareTicket prepareTicket)
    {
        return (ticket.WinLost, ticket.StatusId, prepareTicket.StatusId);
    }

    private void UpdateWinLostValues(Ticket ticket, PrepareTicket prepareTicket, bool isRecovery)
    {
        var winlost = isRecovery
            ? prepareTicketService.RecoverWinLost(ticket, prepareTicket)
            : prepareTicketService.CreateWinLost(ticket, prepareTicket);

        UpdateWinLost(ticket, winlost);
    }

    private void UpdateCommissionValues(Ticket ticket, PrepareTicket prepareTicket, bool isRecovery)
    {
        var commission = isRecovery
            ? prepareTicketService.RecoverCommission(ticket, prepareTicket)
            : prepareTicketService.CreateCommission(ticket, prepareTicket);

        UpdateCommission(ticket, commission);
    }

    private static void UpdateWinLost(Ticket ticket, Winlost winlost)
    {
        ticket.AWinLost = winlost.AWinLost;
        ticket.MWinLost = winlost.MWinLost;
        ticket.SWinLost = winlost.SWinLost;
        ticket.WinLost = winlost.WinLost;
    }

    private static void UpdateCommission(Ticket ticket, Commission commission)
    {
        ticket.PlayerComm = commission.PlayerComm;
        ticket.Comm = commission.Comm;
        ticket.AComm = commission.AComm;
        ticket.SComm = commission.SComm;
    }

    private static void UpdateStatusInfo(
        Ticket ticket,
        PrepareTicket prepareTicket,
        decimal? oldWinLost,
        byte oldStatusId,
        byte newStatusId)
    {
        ticket.Ruben = Ruben.Default;
        ticket.IsUpdateSettlementTime = IsUpdateSettlementTime(oldWinLost, ticket.WinLost, oldStatusId, newStatusId);
        ticket.StatusId = newStatusId;
        ticket.Status = prepareTicket.Status;
        ticket.ExtraInfo = prepareTicket.ExtraInfo;
    }

    private static bool IsUpdateSettlementTime(decimal? oldWinLost, decimal? newWinLost, byte oldStatusId, byte newStatusId)
        => oldWinLost != newWinLost || oldStatusId != newStatusId;

    private static void UpdateMaxPayoutInfo(Ticket ticket, PrepareTicket prepareTicket)
    {
        ticket.IsApplyMaxPayout = prepareTicket.IsApplyMaxPayout;
        ticket.MaxWinLimit = prepareTicket.MaxWinLost;
    }
}