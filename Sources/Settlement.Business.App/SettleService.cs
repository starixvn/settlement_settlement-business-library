using Microsoft.Extensions.Hosting;
using Settlement.Business.Services;

namespace Settlement.Business.App;

public class SettleService(
    MatchService matchService,
    ProcessService processService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Testing();
    }

    private async Task Testing()
    {
        //var request =
        string[] siteDbServer = { "DBTXN", "DBSID01" }; //, "DBSID01", "DBSID02"
        var request = new InputRequest
        {
            MatchId = 83541410,
            Action = "Cal",
            ActionId = 81000
        };

        var (resources, _) = await matchService.GetResourceHappy5(request.MatchId, request.Action);
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var tasks = siteDbServer
            .Select(site => processService.Settle(request, site, resources.ToList())).ToList();

        var results = await Task.WhenAll(tasks.ToArray());
        var resultFailed = results.Where(r => r.ErrCode > 0).ToList();

        watch.Stop();
        var log = $"Total Time execute: {watch.ElapsedMilliseconds}\r\n";
        Console.WriteLine(log);

        Console.WriteLine(string.Join(" | ", resultFailed.Select(r => r.ErrMessage).ToArray()));
    }
}