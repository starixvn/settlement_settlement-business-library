using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Models;

namespace Settlement.Business.Services;

public interface GenerateTicketService
{
    Task<List<Ticket>> Happy5(List<Ticket> tickets, List<Resources> resources);
}

public class GenerateTicketServiceImp(
    ModdsService moddsService,
    WinLimitService winLimitService,
    PrepareTicketService prepareTicketService) : GenerateTicketService
{
    private const byte DefaultRuben = 1;

    public async Task<List<Ticket>> Happy5(List<Ticket> tickets, List<Resources> resources)
    {
        tickets.ForEach(item =>
        {
            var resource = resources.Find(r => r.ProfileId == item.ProfileId) ?? new Resources();
            item.Modds = moddsService.Happy5(item.ProfileId, item.BetTeam, resource);
            item.OddsType = resource.OddsType;
            item.ActualPayout = GetActualPayout(item.Modds, item.Odds, item.OddsType, item.ActualStake, item.Stake);
        });
        var maxWinLimit = (await winLimitService.GetMaxWinLimit(tickets)).ToList();

        tickets.ForEach(item => SetValue(item, maxWinLimit));

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

    private static decimal Odds(decimal odds, short oddsType)
    {
        if (odds < 0)
        {
            return 1;
        }

        return oddsType == 1 ? odds - 1 : odds;
    }

    private void SetValue(Ticket ticket, List<WinLimit> maxWinLimit)
    {
        var prepareTicket = prepareTicketService.CreateValue(ticket, maxWinLimit);
        ticket.IsApplyMaxPayout = prepareTicket.IsApplyMaxPayout;
        ticket.MaxWinLimit = prepareTicket.MaxWinLost;
        ticket.StatusWinLost = prepareTicket.StatusWinLost;
        ticket.ExtraInfo = prepareTicket.ExtraInfo;

        if (prepareTicket.IsUpdated)
        {
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
            ticket.SettlementTime = prepareTicket.SettlementTime;
            ticket.StatusId = prepareTicket.StatusId;
            ticket.Status = prepareTicket.Status;
        }
    }
}