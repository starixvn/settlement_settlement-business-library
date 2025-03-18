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
    public int Happy5(int profileId, string betTeam, Resources resource)
    {
        var mainValue = moddsValueService.GetValueByProfileIdOfHappy5(profileId, resource.Result1, resource.Result2);

        if (HappyFiveConstants.Evaluate.TryGetValue((profileId, betTeam), out var evaluate))
        {
            return evaluate(mainValue) ? 1 : -1;
        }

        return -1;
    }

    public int VirtualRacing(Ticket ticket, int sportType, byte racers, IEnumerable<Outright> outright)
    {
        var correctOutright = outrightResultService.CorrectOutrightResult(sportType, racers, outright).ToList();
        var isExisted = IsWinBetOption(ticket.BetType)
            ? correctOutright.Exists(x => x.Orid == ticket.OddsId && x.Placing == 1)
            : correctOutright.Exists(x => x.Orid == ticket.OddsId);

        return isExisted ? 1 : -1;
    }

    private static bool IsWinBetOption(short betType)
    {
        return betType is BetTypes.RacingWin or BetTypes.KironWin;
    }
}