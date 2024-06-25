using Polly;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Requests;
using System.Text.Json;

namespace Settlement.Business.Services.HappyFive;

public interface ProcessService
{
    Task<Result> ProcessAsync(TicketRequest request, List<Resources> resources);
}

public class ProcessServiceImp(
    TicketService ticketService,
    CommandService generateTicketService,
    InsertHistoryData insertHistoryData) : ProcessService
{
    public async Task<Result> ProcessAsync(TicketRequest request, List<Resources> resources)
    {
        var result = new Result();

        await Policy
            .HandleResult<bool>(flag => !flag)
            .WaitAndRetryAsync(Retry.Count,
                _ => TimeSpan.FromSeconds(Retry.Time))
            .ExecuteAsync(async () =>
            {
                result = string.Equals(request.Action, ActionName.Calculate, StringComparison.OrdinalIgnoreCase)
                    ? await Calc(request, resources)
                    : await Refund(request, resources);
                return result.IsSuccessful;
            });

        return result;
    }

    private async Task<Result> Calc(TicketRequest request, List<Resources> resources)
    {
        return await TryCatch(request, async () =>
        {
            var tickets = await ticketService.GetTickets(request);
            var ticketsAfterCalculate = await generateTicketService.Calc(tickets, resources);

            var ticketsJson = JsonSerializer.Serialize(ticketsAfterCalculate);
            var updateRequest = CreateUpdateRequest(request, ticketsJson);
            var result = await ticketService.UpdateTicket(updateRequest);

            if (result.IsSuccessful)
            {
                await insertHistoryData.InsertHappy5(tickets, request.AdminId);
            }

            return result;
        });
    }

    private async Task<Result> Refund(TicketRequest request, List<Resources> resources)
    {
        return await TryCatch(request, async () =>
        {
            var tickets = await ticketService.GetTickets(request);
            var ticketsAfterRefund = generateTicketService.Refund(tickets, resources);
            var ticketsJson = JsonSerializer.Serialize(ticketsAfterRefund);
            var updateRequest = CreateUpdateRequest(request, ticketsJson);

            return await ticketService.UpdateTicket(updateRequest);
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
            var tickets = $"MinCustId: {request.MinCustId}, MaxCustId: {request.MaxCustId}, MinCustId_MinTransId: {request.MinCustId_MinTransId}, "
                              + $"MinCustId_MaxTransId: {request.MinCustId_MaxTransId}, MaxCustId_MinTransId: {request.MaxCustId_MinTransId}, MaxCustId_MaxTransId: {request.MaxCustId_MaxTransId}";
            return new Result
            {
                ErrCode = 1,
                ErrMessage = $"Match Id: {request.MatchId} - SiteDb: {request.DbServer} - {tickets} - Message: {exception.Message}"
            };
        }
    }

    private static UpdateTicketRequest CreateUpdateRequest(TicketRequest request, string ticketsJson)
    {
        return new UpdateTicketRequest
        {
            MatchId = request.MatchId,
            TransJson = ticketsJson,
            Action = request.Action,
            ActionId = request.ActionId,
            DbServer = request.DbServer
        };
    }
}