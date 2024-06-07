using Microsoft.Extensions.Hosting;
using Settlement.Business.Core.Models;
using Settlement.Business.Services;

namespace Settlement.Business.App;

public class SettleService(
    MatchService matchService,
    ProcessService processService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var result = await Testing();
    }

    private async Task<Result> Testing()
    {
        //var request =
        string[] siteDbServer = { "DBTXN" }; //, "DBSID01", "DBSID02"
        var request = new InputRequest
        {
            MatchId = 83539554,
            Action = "CAL",
        };

        var resources = await matchService.GetResourceHappy5(request.MatchId, request.Action);
        var tasks = siteDbServer
            .Select(site => processService.Settle(request, site, resources)).ToList();

        var results = await Task.WhenAll(tasks.ToArray());
        var resultFailed = results.Where(r => r.ErrCode > 0).ToList();

        return new Result
        {
            ErrCode = resultFailed.Count,
            ErrMessage = string.Join(" | ", resultFailed.Select(r => r.ErrMessage).ToArray())
        };
    }
}