using Settlement.Business.Core.Constants;

namespace Settlement.Business.Constants;

public static class HappyFiveConstants
{
    public const int FirstProfileId = 8101;

    public const int SecondProfileId = 8102;

    public const int ThirdProfileId = 8103;

    public const int FourthProfileId = 8104;

    public const int FifthProfileId = 8105;

    public static IReadOnlyDictionary<(int, string), Func<int, bool>> Evaluate => EvaluateMapping;

    private static readonly Dictionary<(int, string), Func<int, bool>> EvaluateMapping = new()
    {
        { (8101, BetTeam.B), value => value >= 53 },
        { (8101, BetTeam.S), value => value < 53 },
        { (8102, BetTeam.O), value => value == 1 },
        { (8102, BetTeam.E), value => value == 0 },
        { (8103, BetTeam.SP), value => value is >= 15 and <= 33 },
        { (8103, BetTeam.SU), value => value is >= 34 and <= 52 },
        { (8103, BetTeam.AU), value => value is >= 53 and <= 71 },
        { (8103, BetTeam.WI), value => value is >= 72 and <= 90 },
        { (8104, BetTeam.MO), value => value > 0 },
        { (8104, BetTeam.ME), value => value < 0 },
        { (8105, BetTeam.BO), value => value >= 53 && IsOdd(value) },
        { (8105, BetTeam.BE), value => value >= 53 && value % 2 == 0 },
        { (8105, BetTeam.SO), value => value < 53 && IsOdd(value) },
        { (8105, BetTeam.SE), value => value < 53 && value % 2 == 0 }
    };

    private static bool IsOdd(int x)
    {
        return x % 2 != 0;
    }
}