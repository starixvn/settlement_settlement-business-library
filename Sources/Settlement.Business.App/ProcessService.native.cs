using Microsoft.Extensions.Configuration;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Requests;
using Settlement.Business.Services;
using Settlement.Business.Services.Racing;

namespace Settlement.Business.App;

public interface ProcessService
{
    Task<Result> Settle(InputRequest request, string site, List<Outright> outright, List<Resources> resources);
}

public class ProcessServiceImp(MatchService matchService,
    BatchTicketsService batchTicketsService,
    IConfiguration configuration) : ProcessService
{
    public async Task<Result> Settle(InputRequest request, string site, List<Outright> outright, List<Resources> resources)
    {
        try
        {
            var result = await matchService.LockMatch(request.EventId, 0, site, LockMatchType.VirtualRacing);
            if (result.IsSuccessful)
            {
                var batchTicketRequest = CreateBatchTicketRequest(request, site, resources);
                result = await batchTicketsService.Settle(batchTicketRequest, outright);
                await matchService.LockMatch(request.EventId, 0, site, LockMatchType.VirtualRacing);
            }

            return result;
        }
        catch (Exception exception)
        {
            var errorMessage = $"Match Id = {request.EventId} - SiteDb = {site} - {exception.Message}";
            await matchService.LockMatch(request.EventId, 0, site, LockMatchType.VirtualRacing);

            return new Result
            {
                ErrCode = 1,
                ErrMessage = errorMessage
            };
        }
    }

    private BatchTicketRequest CreateBatchTicketRequest(InputRequest request, string site, List<Resources> resources)
    {
        return new BatchTicketRequest
        {
            MatchId = request.EventId,
            AdminId = request.AdminId,
            SportType = request.SportType,
            Racers = request.Racers,
            Action = request.Action,
            ActionId = request.ActionId,
            Resources = resources,
            DbServer = site,
            LastCustId = 0,
            LastTransId = 0,
            BatchSize = Convert.ToInt32(configuration.GetSection("MaxBatchSize").Value),
            BatchTotal = Convert.ToInt32(configuration.GetSection("MaxWorker").Value)
        };
    }
}