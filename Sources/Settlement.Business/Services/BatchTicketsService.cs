using Settlement.Business.Models;
using Settlement.Business.Requests;

namespace Settlement.Business.Services;

public interface BatchTicketsService
{
    Task SettleByBatch(BatchTicketRequest request);
}

public class BatchTicketsServiceImp(
    TicketService ticketService,
    CommandService commandService) : BatchTicketsService
{
    public async Task SettleByBatch(BatchTicketRequest request)
    {
        var lc = true;
        while (lc)
        {
            var batches = await GetBatches(request);
            if (batches.Count > 0)
            {
                var result = await commandService.Settle(request, batches);
            }

            lc = request is { LastTransId: > 0, LastCustId: > 0 };
        }
    }

    private async Task<List<BetTranBatch>> GetBatches(BatchTicketRequest request)
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