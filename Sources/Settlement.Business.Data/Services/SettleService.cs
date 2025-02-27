using Settlement.Business.Core.Models;
using Settlement.Business.Core.Requests;

namespace Settlement.Business.Data.Services;

public interface SettleService
{
    Task<Result> Calc(TicketRequest request, List<Resources> resources);

    Task<Result> Calc(TicketRequest request, List<Outright> outright, List<Resources> resources);

    Task<Result> Refund(TicketRequest request);
}

public class SettleServiceImp(
    TicketService ticketService,
    GenerateSettleTicketService generateSettleTicketService,
    HistoryData historyData) : SettleService
{
    public async Task<Result> Calc(TicketRequest request, List<Resources> resources)
    {
        var tickets = new List<Ticket>();

        var result = await TryCatch(request, async () =>
        {
            tickets = await ticketService.GetTickets(request);
            var ticketsAfterCalculate = await generateSettleTicketService.Calc(tickets, resources);

            return await ticketService.UpdateTicket(request, ticketsAfterCalculate);
        });

        if (tickets.Count > 0 && result.IsSuccessful)
        {
            return await historyData.Insert(request, tickets);
        }

        return result;
    }

    public async Task<Result> Calc(TicketRequest request, List<Outright> outright, List<Resources> resources)
    {
        var tickets = new List<Ticket>();

        var result = await TryCatch(request, async () =>
        {
            tickets = await ticketService.GetTickets(request);
            var ticketsAfterCalculate = await generateSettleTicketService.Calc(tickets, outright, resources);

            return await ticketService.UpdateTicket(request, ticketsAfterCalculate);
        });

        if (tickets.Count > 0 && result.IsSuccessful)
        {
            return await historyData.Insert(request, tickets);
        }

        return result;
    }

    public async Task<Result> Refund(TicketRequest request)
    {
        return await TryCatch(request, async () =>
        {
            var tickets = await ticketService.GetTickets(request);
            var ticketsAfterRefund = generateSettleTicketService.Refund(tickets);

            return await ticketService.UpdateTicket(request, ticketsAfterRefund);
        });
    }

    private static async Task<Result> TryCatch(TicketRequest request, Func<Task<Result>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception exception)
        {
            var tickets =
                $"MinCustId: {request.MinCustId}, MaxCustId: {request.MaxCustId}, MinCustId_MinTransId: {request.MinCustId_MinTransId}, "
                + $"MinCustId_MaxTransId: {request.MinCustId_MaxTransId}, MaxCustId_MinTransId: {request.MaxCustId_MinTransId}, MaxCustId_MaxTransId: {request.MaxCustId_MaxTransId}";
            return new Result
            {
                ErrCode = 1,
                ErrMessage =
                    $"Match Id: {request.MatchId} - SiteDb: {request.DbServer} - {tickets} - Message: {exception.Message}"
            };
        }
    }
}