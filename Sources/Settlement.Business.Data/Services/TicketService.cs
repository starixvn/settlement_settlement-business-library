using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Core.Requests;
using Settlement.Business.Data.Criteria;
using System.Text.Json;

namespace Settlement.Business.Data.Services;

public interface TicketService
{
    TicketRequest CreateTicketRequest(BatchTicketRequest batchTicketRequest, TicketBatch batch);

    Task<List<TicketBatch>> GetBatches(BatchTicketRequest request);

    Task<List<Ticket>> GetTickets(TicketRequest request);

    Task<Result> UpdateTicket(TicketRequest request, List<Ticket> tickets);
}

public class TicketServiceImp(IDynamicRepository repository) : TicketService
{
    public TicketRequest CreateTicketRequest(BatchTicketRequest batchTicketRequest, TicketBatch batch)
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
        var result = (await repository.FetchAsync<Ticket>(criteria)).ToList();

        return result;
    }

    public async Task<Result> UpdateTicket(TicketRequest request, List<Ticket> tickets)
    {
        var ticketsJson = JsonSerializer.Serialize(tickets);
        var (eventTable, hasNoBalanceNegative, cwlBettype) = GetEventAndNegativeBalance(request.SportType);

        var criteria = new UpdateOtherSportsCriteria
        {
            MatchId = request.MatchId,
            Action = request.Action,
            ActionId = request.ActionId,
            DbServer = request.DbServer,
            TransJson = ticketsJson,
            EventTable = eventTable,
            HasNoBalanceNegative = hasNoBalanceNegative,
            Is3rd = true,
            IsCalculateBalance = true,
            BalanceType = 4,
            CWLBettype = cwlBettype,
            SettleByApp = true
        };

        await repository.ExecuteAsync(criteria);

        return new Result
        {
            ErrCode = criteria.ErrCode,
            ErrMessage = criteria.ErrMessage
        };
    }

    private static (byte, bool, short) GetEventAndNegativeBalance(int sportType)
    {
        return IsVirtualSport(sportType)
            ? (EventTable.VirtualRacing, false, (short)1201)
            : (EventTable.Match, true, (short)81);
    }

    private static bool IsVirtualSport(int sportType)
    {
        return sportType is SportType.VirtualDogs or SportType.VirtualHorses
            or SportType.VirtualSteepleChase
            or SportType.VirtualHarnessRacing
            or SportType.VirtualMotorRacing;
    }
}