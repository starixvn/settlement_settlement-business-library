using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface OutrightResultService
{
    IEnumerable<Outright> CorrectOutrightResult(int sportType, byte racers, IEnumerable<Outright> outright);
}

public class OutrightResultServiceImp : OutrightResultService
{
    public IEnumerable<Outright> CorrectOutrightResult(int sportType, byte racers, IEnumerable<Outright> outright)
    {
        int[] validPlacings = IsCheckedRacersWithSportType(sportType) && racers == 6
            ? [1, 2]
            : [1, 2, 3];

        return outright.Where(x => validPlacings.Contains(x.Placing));
    }

    private static bool IsCheckedRacersWithSportType(int sportType)
    {
        return sportType is SportType.VirtualDogs or SportType.VirtualSteepleChase or SportType.VirtualHarnessRacing or SportType.VirtualMotorRacing;
    }
}