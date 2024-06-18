using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Requests;

namespace Settlement.Business.Services;

public interface TicketService
{
    Task<List<TicketBatch>> GetBatches(BatchTicketRequest request);

    Task<List<Ticket>> GetTickets(TicketRequest request);

    Task<Result> UpdateTicket(UpdateTicketRequest request);
}

public class TicketServiceImp(
    IDynamicRepository repository) : TicketService
{
    public async Task<List<TicketBatch>> GetBatches(BatchTicketRequest request)
    {
        var batches = await GetBatchTicket(request);
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

    private async Task<List<TicketBatch>> GetBatchTicket(BatchTicketRequest request)
    {
        var resourceIds = string.Join(',', request.Resources.Select(x => x.ResourceId).Distinct().ToArray());

        var criteria = new GetBatchTicketCriteria
        {
            MatchId = request.MatchId,
            SportType = request.SportType,
            Resources = resourceIds,
            Action = request.Action,
            LastCustId = request.LastCustId,
            LastTransId = request.LastTransId,
            BatchSize = request.BatchSize,
            BatchTotal = request.BatchTotal,
            DbServer = request.DbServer
        };
        return (await repository.FetchAsync<TicketBatch>(criteria)).ToList();
    }

    public async Task<List<Ticket>> GetTickets(TicketRequest request)
    {
        var criteria = new GetTicketCriteria
        {
            MatchId = request.MatchId,
            SportType = request.SportType,
            Resources = request.Resources,
            Action = request.Action,
            DbServer = request.DbServer,

            MinCustId = request.MinCustId,
            MinCustId_MinTransId = request.MinCustId_MinTransId,
            MinCustId_MaxTransId = request.MinCustId_MaxTransId,

            MaxCustId = request.MaxCustId,
            MaxCustId_MinTransId = request.MaxCustId_MinTransId,
            MaxCustId_MaxTransId = request.MaxCustId_MaxTransId
        };
        return (await repository.FetchAsync<Ticket>(criteria)).ToList();
    }

    public async Task<Result> UpdateTicket(UpdateTicketRequest request)
    {
        var criteria = new UpdateOtherSportsCriteria
        {
            MatchId = request.MatchId,
            TransJson = request.TransJson,
            Action = request.Action,
            ActionId = request.ActionId,
            EventTable = 1,
            Is3rd = true,
            IsCalculateBalance = true,
            BalanceType = 81,
            CWLBettype = 4,
            SettleByApp = true,
            DbServer = request.DbServer
        };
        await repository.ExecuteAsync(criteria);

        return new Result
        {
            ErrCode = criteria.ErrCode,
            ErrMessage = criteria.ErrMessage
        };
    }
}