using Settlement.Business.Constants;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface ModdsService
{
    int Happy5(int profileId, string betTeam, Resources resource);

    int VirtualRacing(Ticket ticket, int sportType, byte racers, IEnumerable<Outright> outright);
}

public class ModdsServiceImp(
    ModdsValueService moddsValueService,
    OutrightResultService outrightResultService) : ModdsService
{
    private const int WinningModds = 1;

    private const int LosingModds = -1;

    public int Happy5(int profileId, string betTeam, Resources resource)
    {
        var mainValue = moddsValueService.GetValueByProfileIdOfHappy5(
            profileId,
            resource.Result1,
            resource.Result2);

        return IsHappy5Winner(profileId, betTeam, mainValue)
            ? WinningModds
            : LosingModds;
    }

    private static bool IsHappy5Winner(int profileId, string betTeam, int mainValue)
    {
        return HappyFiveConstants.Evaluate.TryGetValue(
            (profileId, betTeam),
            out var evaluate) && evaluate(mainValue);
    }

    public int VirtualRacing(Ticket ticket, int sportType, byte racers, IEnumerable<Outright> outright)
    {
        var correctOutright = outrightResultService
            .CorrectOutrightResult(sportType, racers, outright)
            .ToList();

        return IsVirtualRacingWinner(ticket, correctOutright)
            ? WinningModds
            : LosingModds;
    }

    private static bool IsVirtualRacingWinner(
        Ticket ticket,
        IReadOnlyCollection<Outright> correctOutright)
    {
        var isWinBetType = IsWinBetType(ticket.BetType);

        return isWinBetType
            ? correctOutright.Any(x => x.Orid == ticket.OddsId && x.Placing == 1)
            : correctOutright.Any(x => x.Orid == ticket.OddsId);
    }

    private static bool IsWinBetType(short betType) => betType is BetTypes.RacingWin or BetTypes.KironWin;
}