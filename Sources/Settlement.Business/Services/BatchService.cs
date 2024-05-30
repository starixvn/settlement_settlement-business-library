using Settlement.Business.Criteria;
using Settlement.Business.Models;
using Settlement.Business.Requests;

namespace Settlement.Business.Services;

public interface BatchService
{
    Task<IEnumerable<BetTranBatch>> BatchTranBatch(BatchRequest request);
}

public class BatchServiceImp(IDynamicRepository repository) : BatchService
{
    public async Task<IEnumerable<BetTranBatch>> BatchTranBatch(BatchRequest request)
    {
        var criteria = new GetBatchCriteria
        {
            MatchId = request.MatchId,
            SportType = request.SportType,
            Resources = request.Resources,
            Action = request.Action,
            LastCustId = request.LastCustId,
            LastTransId = request.LastTransId,
            BatchSize = request.BatchSize,
            BatchTotal = request.BatchTotal
        };
        return await repository.FetchAsync<BetTranBatch>(criteria);
    }
}