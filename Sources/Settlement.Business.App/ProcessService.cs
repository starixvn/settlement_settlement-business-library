using Settlement.Business.Core.Models;
using Settlement.Business.Models;
using Settlement.Business.Requests;
using Settlement.Business.Services;
using BusinessConstants = Settlement.Business.Core.Constants;

namespace Settlement.Business.App;

public interface ProcessService
{
    Task<Result> Settle(InputRequest request, string site, List<Resources> resources);
}

public class ProcessServiceImp(Services.MatchService matchService,
    BatchTicketsService batchTicketsService) : ProcessService
{
    public async Task<Result> Settle(InputRequest request, string site, List<Resources> resources)
    {
        try
        {
            var unLockMatch = await matchService.LockMatch(request.MatchId, 0, site);

            var lockMatch = await matchService.LockMatch(request.MatchId, 1, site);
            if (lockMatch.IsSuccessful)
            {
                var batchTicketRequest = CreateBatchTicketRequest(request, site, resources);
                await batchTicketsService.SettleByBatch(batchTicketRequest);
            }

            await matchService.LockMatch(request.MatchId, 0, site);

            return new Result
            {
                ErrCode = lockMatch.ErrCode,
                ErrMessage = lockMatch.ErrMessage
            };
        }
        catch (Exception e)
        {
            var unLockMatch = await matchService.LockMatch(request.MatchId, 0, site);
            Console.WriteLine(e);
            throw;
        }
    }

    private static BatchTicketRequest CreateBatchTicketRequest(InputRequest request, string site, List<Resources> resources)
    {
        return new BatchTicketRequest
        {
            MatchId = request.MatchId,
            SportType = request.SportType,
            Action = request.Action,
            Resources = resources,
            DbServer = site,
            LastCustId = 0,
            LastTransId = 0,
            BatchSize = BusinessConstants.Settings.MaxBatchSize,
            BatchTotal = BusinessConstants.Settings.MaxWorker
        };
    }
}