using Settlement.Business.Core.Models;
using Settlement.Business.Requests;

namespace Settlement.Business.Services.HappyFive;

public interface BatchTicketsService
{
    Task<Result> Settle(BatchTicketRequest request);
}

public class BatchTicketsServiceImp(
    TicketService ticketService,
    ProcessService processService) : BatchTicketsService
{
    public async Task<Result> Settle(BatchTicketRequest request)
    {
        var listResult = new List<Result>();

        var flag = true;
        while (flag)
        {
            var batches = await ticketService.GetBatches(request);
            if (batches.Count > 0)
            {
                var result = await ExecuteBatches(request, batches);
                if (result.ErrCode > 0)
                {
                    listResult.Add(result);
                }
            }

            flag = request is { LastTransId: > 0, LastCustId: > 0 };
        }

        return new Result
        {
            ErrCode = listResult.Count,
            ErrMessage = string.Join(" | ", listResult.Select(r => r.ErrMessage).ToArray())
        };
    }

    private async Task<Result> ExecuteBatches(BatchTicketRequest batchTicketRequest, List<TicketBatch> batches)
    {
        var tasks = new List<Task<Result>>();

        foreach (var batch in batches)
        {
            var request = CreateTicketRequest(batchTicketRequest, batch);

            var task = processService.ProcessAsync(request, batchTicketRequest.Resources);
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
            ActionId = batchTicketRequest.ActionId,
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