using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Core.Requests;

namespace Settlement.Business.Data.Services.Products;

public interface RacingProcessService
{
    Task<Result> Settle(BatchTicketRequest request, IEnumerable<Outright> outright);
}

public class RacingProcessServiceImp(
    TicketService ticketService,
    SettleService settleService) : RacingProcessService
{
    public async Task<Result> Settle(BatchTicketRequest request, IEnumerable<Outright> outright)
    {
        var listResult = new List<Result>();
        var flag = true;
        while (flag)
        {
            var batches = await ticketService.GetBatches(request);

            if (batches.Count > 0)
            {
                var result = await ExecuteBatches(request, batches, outright);
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

    private async Task<Result> ExecuteBatches(BatchTicketRequest batchTicketRequest, List<TicketBatch> batches, IEnumerable<Outright> outright)
    {
        var tasks = new List<Task<Result>>();
        foreach (var batch in batches)
        {
            var request = ticketService.CreateTicketRequest(batchTicketRequest, batch);

            var task = ProcessAsync(request, outright, batchTicketRequest.Resources, batchTicketRequest.Racers);
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

    private async Task<Result> ProcessAsync(TicketRequest request, IEnumerable<Outright> outright, List<Resources> resources, byte racers)
    {
        var result = new Result();

        await Policy
            .HandleResult<bool>(flag => !flag)
            .WaitAndRetryAsync(Retry.Count,
                _ => TimeSpan.FromSeconds(Retry.Time))
            .ExecuteAsync(async () =>
            {
                result = string.Equals(request.Action, ActionName.Calculate, StringComparison.OrdinalIgnoreCase)
                    ? await settleService.Calc(request, outright.ToList(), resources, racers)
                    : await settleService.Refund(request);
                return result.IsSuccessful;
            });

        return result;
    }
}