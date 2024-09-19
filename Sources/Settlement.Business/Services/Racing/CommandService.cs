using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using static Settlement.Business.Core.Constants.Status;

namespace Settlement.Business.Services.Racing;

public interface CommandService
{
    Task<List<Ticket>> Calc(List<Ticket> tickets, List<Resources> resources);

    List<Ticket> Refund(List<Ticket> tickets, List<Resources> resources);
}

public class CommandServiceImp(
    ModdsService moddsService,
    WinLimitService winLimitService,
    PrepareTicketService prepareTicketService) : CommandService
{
    private const byte DefaultRuben = 1;

    public async Task<List<Ticket>> Calc(List<Ticket> tickets, List<Resources> resources)
    {
        tickets.ForEach(item =>
        {
            var resource = resources.Find(r => r.ProfileId == item.ProfileId) ?? new Resources();
            //item.Modds = moddsService.Happy5(item.ProfileId, item.BetTeam, resource);
            item.OddsType = resource.OddsType;
            //item.ActualPayout = GetActualPayout(item.Modds, item.Odds, item.OddsType, item.ActualStake, item.Stake);
        });
        var maxWinLimit = (await winLimitService.GetMaxWinLimit(tickets)).ToList();

        tickets.ForEach(item => SetValue(item, maxWinLimit));

        return tickets;
    }

    public List<Ticket> Refund(List<Ticket> tickets, List<Resources> resources)
    {
        tickets.ForEach(item =>
        {
            item.WinLost = item.ActualStake;
            item.AWinLost = 0;
            item.MWinLost = 0;
            item.SWinLost = 0;
            item.PlayerComm = 0;
            item.Comm = 0;
            item.AComm = 0;
            item.SComm = 0;
            item.Ruben = DefaultRuben;
            item.StatusWinLost = StatusWinlost.Refund;
            item.StatusId = StatusId.Refund;
            item.Status = StatusName.Refund;
        });

        return tickets;
    }

    private static decimal? GetActualPayout(decimal modds, decimal odds, short oddsType, decimal actualStake, decimal stake)
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

    private void SetValue(Ticket ticket, List<WinLimit> maxWinLimit)
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

            ticket.Ruben = DefaultRuben;
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