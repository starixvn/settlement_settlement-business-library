using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.Data.Services;

public interface GenerateSettleTicketService
{
    Task<List<Ticket>> Calc(List<Ticket> tickets, List<Resources> resources);

    Task<List<Ticket>> Calc(List<Ticket> tickets, IEnumerable<Outright> outright, List<Resources> resources, int sportType, byte racers);

    List<Ticket> Refund(List<Ticket> tickets);
}

public class GenerateSettleTicketServiceImp(
    ModdsService moddsService,
    WinLimitService winLimitService,
    TicketValueService ticketValueService) : GenerateSettleTicketService
{
    public async Task<List<Ticket>> Calc(List<Ticket> tickets, List<Resources> resources)
    {
        tickets.ForEach(item =>
        {
            var resource = resources.Find(r => r.ProfileId == item.ProfileId) ?? new Resources();
            item.Modds = moddsService.Happy5(item.ProfileId, item.BetTeam, resource);
            item.OddsType = resource.OddsType;
            item.ActualPayout = ticketValueService.GetActualPayout(item.Modds, item.Odds, item.OddsType, item.ActualStake, item.Stake);
        });
        var maxWinLimit = (await winLimitService.GetMaxWinLimit(tickets)).ToList();

        tickets.ForEach(item => ticketValueService.SetValue(item, maxWinLimit));

        return tickets;
    }

    public async Task<List<Ticket>> Calc(List<Ticket> tickets, IEnumerable<Outright> outright, List<Resources> resources, int sportType, byte racers)
    {
        tickets.ForEach(item =>
        {
            var resource = resources.Find(r => r.ProfileId == item.ProfileId) ?? new Resources();
            item.Modds = moddsService.VirtualRacing(item, sportType, racers, outright);
            item.OddsType = resource.OddsType;
            item.ActualPayout = ticketValueService.GetActualPayout(item.Modds, item.Odds, item.OddsType, item.ActualStake, item.Stake);
        });
        var maxWinLimit = (await winLimitService.GetMaxWinLimit(tickets)).ToList();

        tickets.ForEach(item => ticketValueService.SetValue(item, maxWinLimit));

        return tickets;
    }

    public List<Ticket> Refund(List<Ticket> tickets)
    {
        tickets.ForEach(ticket => ticketValueService.SetValue(ticket, false));

        return tickets;
    }
}