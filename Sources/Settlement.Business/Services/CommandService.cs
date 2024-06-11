using Settlement.Business.Core.Models;
using Settlement.Business.Models;
using Settlement.Business.Requests;
using System.Text.Json;

namespace Settlement.Business.Services;

public interface CommandService
{
    Task<Result> Settle(BatchTicketRequest batchTicketRequest, List<TicketBatch> batches);
}

public class CommandServiceImp(
    TicketService ticketService,
    GenerateTicketService generateTicketService,
    InsertHistoryData insertHistoryData) : CommandService
{
    public async Task<Result> Settle(BatchTicketRequest batchTicketRequest, List<TicketBatch> batches)
    {
        var tasks = new List<Task<Result>>();

        foreach (var batch in batches)
        {
            var request = CreateTicketRequest(batchTicketRequest, batch);
            var task = GetAndUpdateTicket(request, batchTicketRequest.Resources);
            tasks.Add(task);
        }

        var results = await Task.WhenAll(tasks);

        var resultFailed = results.Where(r => r.ErrCode > 0).ToList();

        return new Result
        {
            ErrCode = resultFailed.Count,
            ErrMessage = string.Join(" | ", resultFailed.Select(r => r.ErrMessage).ToArray())
        };
    }

    private async Task<Result> GetAndUpdateTicket(TicketRequest request, List<Resources> resources)
    {
        var tickets = await ticketService.GetTickets(request);
        var ticketsAfterCalculate = await generateTicketService.Happy5(tickets, resources);

        var ticketsJson = JsonSerializer.Serialize(ticketsAfterCalculate);
        var updateRequest = CreateUpdateRequest(request, ticketsJson);
        var updateTicketResult = await ticketService.UpdateTicket(updateRequest);
        if (updateTicketResult.IsSuccessful)
        {
            await insertHistoryData.InsertHappy5(tickets, request.AdminId, updateTicketResult);
        }

        return updateTicketResult;
    }

    private static UpdateTicketRequest CreateUpdateRequest(TicketRequest request, string ticketsJson)
    {
        return new UpdateTicketRequest
        {
            MatchId = request.MatchId,
            TransJson = ticketsJson,
            Action = request.Action,
            DbServer = request.DbServer
        };
    }

    private static TicketRequest CreateTicketRequest(BatchTicketRequest batchTicketRequest, TicketBatch batch)
    {
        var resourceIds = string.Join(',', batchTicketRequest.Resources.Select(x => x.ResourceId).Distinct().ToArray());

        return new TicketRequest
        {
            MatchId = batchTicketRequest.MatchId,
            AdminId = batchTicketRequest.AdminId,
            SportType = batchTicketRequest.SportType,
            Resources = resourceIds,
            Action = batchTicketRequest.Action,
            DbServer = batchTicketRequest.DbServer,

            MinCustId = batch.MinCustId,
            MinCustId_MinTransId = batch.MinCustId_MinTransId,
            MinCustId_MaxTransId = batch.MinCustId_MaxTransId,

            MaxCustId = batch.MaxCustId,
            MaxCustId_MinTransId = batch.MaxCustId_MinTransId,
            MaxCustId_MaxTransId = batch.MaxCustId_MaxTransId,
        };
    }
}