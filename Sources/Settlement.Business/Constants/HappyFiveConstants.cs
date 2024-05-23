namespace Settlement.Business.Constants;

public static class HappyFiveConstants
{
    public const int FirstProfileId = 8101;

    public const int SecondProfileId = 8102;

    public const int ThirdProfileId = 8103;

    public const int FourthProfileId = 8104;

    public const int FifthProfileId = 8105;

    public static readonly Dictionary<(int, string), Func<int, bool>> Evaluate = new()
    {
        { (8101, "b"), value => value >= 53 },
        { (8101, "s"), value => value < 53 },
        { (8102, "o"), value => value == 1 },
        { (8102, "e"), value => value == 0 },
        { (8103, "sp"), value => value is >= 15 and <= 33 },
        { (8103, "su"), value => value is >= 34 and <= 52 },
        { (8103, "au"), value => value is >= 53 and <= 71 },
        { (8103, "wi"), value => value is >= 72 and <= 90 },
        { (8104, "mo"), value => value > 0 },
        { (8104, "me"), value => value < 0},
        { (8105, "bo"), value => value >= 53 && value % 2 == 1 },
        { (8105, "be"), value => value >= 53 && value % 2 == 0},
        { (8105, "so"), value => value < 53 && value % 2 == 1},
        { (8105, "se"), value => value < 53 && value % 2 == 0},
    };
}