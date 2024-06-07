using Settlement.Business.Core.Models;
using Settlement.Business.Requests;

namespace Settlement.Business.Services;

public interface BatchTicketsService
{
    Task<Result> SettleByBatch(BatchTicketRequest request);
}

public class BatchTicketsServiceImp(
    TicketService ticketService,
    CommandService commandService) : BatchTicketsService
{
    public async Task<Result> SettleByBatch(BatchTicketRequest request)
    {
        var listResult = new List<Result>();

        var watch = System.Diagnostics.Stopwatch.StartNew();
        var flag = true;
        while (flag)
        {
            var batches = await GetBatches(request);
            if (batches.Count > 0)
            {
                var result = await commandService.Settle(request, batches);
                if (result.ErrCode > 0)
                {
                    listResult.Add(result);
                }
            }

            flag = request is { LastTransId: > 0, LastCustId: > 0 };
        }
        watch.Stop();
        var log = $"Time execute: {watch.ElapsedMilliseconds}\r\n";
        Console.WriteLine(log);

        return new Result
        {
            ErrCode = listResult.Count,
            ErrMessage = string.Join(" | ", listResult.Select(r => r.ErrMessage).ToArray())
        };
    }

    private async Task<List<TicketBatch>> GetBatches(BatchTicketRequest request)
    {
        var batches = await ticketService.GetBatchTicket(request);
        var count = batches.Count;
        if (count <= 0)
        {
            request.LastCustId = 0;
            request.LastTransId = 0;
            return batches;
        }

        var latestBatch = batches[count - 1];
        request.LastCustId = latestBatch.MaxCustId;
        request.LastTransId = latestBatch.MaxCustId_MaxTransId;

        return batches;
    }
}