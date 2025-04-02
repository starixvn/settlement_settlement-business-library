using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface OutrightResultService
{
    IEnumerable<Outright> CorrectOutrightResult(int sportType, byte racers, IEnumerable<Outright> outright);
}

public class OutrightResultServiceImp : OutrightResultService
{
    private const byte SixRacers = 6;
    private static readonly int[] TopTwoPlacings = [1, 2];
    private static readonly int[] TopThreePlacings = [1, 2, 3];

    private static readonly HashSet<int> VirtualRacingSportTypes =
    [
        SportType.VirtualDogs,
        SportType.VirtualSteepleChase,
        SportType.VirtualHarnessRacing,
        SportType.VirtualMotorRacing
    ];

    public IEnumerable<Outright> CorrectOutrightResult(int sportType, byte racers, IEnumerable<Outright> outright)
    {
        var validPlacings = GetValidPlacings(sportType, racers);

        return outright.Where(x => validPlacings.Contains(x.Placing));
    }

    private static int[] GetValidPlacings(int sportType, byte racers)
    {
        return IsSpecialVirtualRacing(sportType, racers)
            ? TopTwoPlacings
            : TopThreePlacings;
    }

    private static bool IsSpecialVirtualRacing(int sportType, byte racers)
    {
        return IsVirtualRacingSport(sportType) && racers == SixRacers;
    }

    private static bool IsVirtualRacingSport(int sportType)
    {
        return VirtualRacingSportTypes.Contains(sportType);
    }
}