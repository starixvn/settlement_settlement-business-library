namespace Settlement.Business.Core.Constants;

public class BetTypes
{
    public const int FirstHalfHomeTeamOddEven = 136;

    public const int OneXTwoBackLay = 9538;

    private const int MatchWinnerBackLay = 9539;

    private const int SuperOverWinnerBackLay = 9540;

    public static readonly List<int> BackLayBetType = [OneXTwoBackLay, MatchWinnerBackLay, SuperOverWinnerBackLay];
}