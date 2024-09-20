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
            var request = ticketService.CreateTicketRequest(batchTicketRequest, batch);

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
}