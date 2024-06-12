using Settlement.Business.Core.Models;
using Settlement.Business.Requests;
using Settlement.Business.Services;
using BusinessConstants = Settlement.Business.Core.Constants;

namespace Settlement.Business.App;

public interface ProcessService
{
    Task<Result> Settle(InputRequest request, string site, List<Resources> resources);
}

public class ProcessServiceImp(MatchService matchService,
    BatchTicketsService batchTicketsService) : ProcessService
{
    public async Task<Result> Settle(InputRequest request, string site, List<Resources> resources)
    {
        try
        {
            var result = await matchService.LockMatch(request.MatchId, 1, site);
            if (result.IsSuccessful)
            {
                var batchTicketRequest = CreateBatchTicketRequest(request, site, resources);
                result = await batchTicketsService.SettleByBatch(batchTicketRequest);
                await matchService.LockMatch(request.MatchId, 0, site);
            }

            return result;
        }
        catch (Exception exception)
        {
            var errorMessage = $"Match Id = {request.MatchId} - SiteDb = {site} - {exception.Message}";
            await matchService.LockMatch(request.MatchId, 0, site);

            return new Result
            {
                ErrCode = 1,
                ErrMessage = errorMessage
            };
        }
    }

    private static BatchTicketRequest CreateBatchTicketRequest(InputRequest request, string site, List<Resources> resources)
    {
        return new BatchTicketRequest
        {
            MatchId = request.MatchId,
            AdminId = request.AdminId,
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