using Newtonsoft.Json;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Models;
using Settlement.Business.Requests;

namespace Settlement.Business.Services;

public interface CommandService
{
    Task<Result> Settle(BatchTicketRequest batchTicketRequest, List<BetTranBatch> batches);
}

public class CommandServiceImp(
    TicketService ticketService,
    GenerateBetTranService generateBetTranService
    ) : CommandService
{
    public async Task<Result> Settle(BatchTicketRequest batchTicketRequest, List<BetTranBatch> batches)
    {
        var tasks = new List<Task<Result>>();

        foreach (var batch in batches)
        {
            if (tasks.Count >= Settings.MaxWorker)
            {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
            }

            var request = CreateRequest(batchTicketRequest, batch);
            var task = GetAndUpdateTicket(request);
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

    private async Task<Result> GetAndUpdateTicket(TicketRequest request)
    {
        var tickets = await ticketService.GetTickets(request);
        var ticketsAfterCalculate = await generateBetTranService.Happy5(tickets);

        var ticketsJson = JsonConvert.SerializeObject(ticketsAfterCalculate);

        //ticketService.UpdateTicket()

        return new Result();
    }

    private static TicketRequest CreateRequest(BatchTicketRequest batchTicketRequest, BetTranBatch batch)
    {
        return new TicketRequest
        {
            MatchId = batchTicketRequest.MatchId,
            SportType = batchTicketRequest.SportType,
            Resources = batchTicketRequest.Resources,
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