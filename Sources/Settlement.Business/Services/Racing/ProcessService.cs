using Polly;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Requests;

namespace Settlement.Business.Services.Racing;

public interface ProcessService
{
    Task<Result> ProcessAsync(TicketRequest request, List<Outright> outright, List<Resources> resources);
}

public class ProcessServiceImp(
    TicketService ticketService,
    SettleService settleService,
    HistoryData historyData) : ProcessService
{
    public async Task<Result> ProcessAsync(TicketRequest request, List<Outright> outright, List<Resources> resources)
    {
        var result = new Result();

        await Policy
            .HandleResult<bool>(flag => !flag)
            .WaitAndRetryAsync(Retry.Count,
                _ => TimeSpan.FromSeconds(Retry.Time))
            .ExecuteAsync(async () =>
            {
                result = string.Equals(request.Action, ActionName.Calculate, StringComparison.OrdinalIgnoreCase)
                    ? await Calc(request, outright, resources)
                    : await Refund(request);
                return result.IsSuccessful;
            });

        return result;
    }

    private async Task<Result> Calc(TicketRequest request, List<Outright> outright, List<Resources> resources)
    {
        var tickets = new List<Ticket>();

        var result = await TryCatch(request, async () =>
        {
            tickets = await ticketService.GetTickets(request);
            var ticketsAfterCalculate = await settleService.Calc(tickets, outright, resources);

            return await ticketService.UpdateTicket(request, ticketsAfterCalculate);
        });

        if (tickets.Count > 0 && result.IsSuccessful)
        {
            return await historyData.Insert(request, tickets);
        }

        return result;
    }

    private async Task<Result> Refund(TicketRequest request)
    {
        return await TryCatch(request, async () =>
        {
            var tickets = await ticketService.GetTickets(request);
            var ticketsAfterRefund = settleService.Refund(tickets);

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