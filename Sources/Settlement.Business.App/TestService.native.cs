using Microsoft.Extensions.Hosting;
using Settlement.Business.Data.Services;

namespace Settlement.Business.App;

public class TestService(
    MatchService matchService,
    ProcessService processService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Testing();
    }

    private async Task Testing()
    {
        const int matchId = 10000084;
        string[] siteDbServer = { "DBSITE03" }; //, "DBSITE01", "DBSITE02"
        var request = new InputRequest
        {
            EventId = matchId,
            Action = "CAL",
            ActionId = 187,
        };

        var (raceInfo, outright, resources) = await matchService.GetRacingInfo(request.EventId);

        request.SportType = raceInfo.SportType;
        request.Racers = raceInfo.Racers;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var tasks = siteDbServer
            .Select(site => processService.Settle(request, site, outright, resources)).ToList();

        var results = await Task.WhenAll(tasks.ToArray());
        var resultFailed = results.Where(r => r.ErrCode > 0).ToList();

        watch.Stop();
        var log = $"Total Time execute: {watch.ElapsedMilliseconds}\r\n";
        Console.WriteLine(log);

        Console.WriteLine(string.Join(" | ", resultFailed.Select(r => r.ErrMessage).ToArray()));
    }
}