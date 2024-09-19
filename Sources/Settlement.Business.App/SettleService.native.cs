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
        const int matchId = 10155241;
        string[] siteDbServer = { "DBTXN" }; //, "DBSID01", "DBSID02"
        var request = new InputRequest
        {
            EventId = matchId,
            Action = "Cal",
            ActionId = 187,
        };

        var (raceInfo, outrights, resources) = await matchService.GetRacingInfo(request.EventId);

        request.SportType = raceInfo.SportType;
        request.Racers = raceInfo.Racers;
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