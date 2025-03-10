namespace Settlement.Business.Core.Constants;

public static class BetTypes
{
    public const int RacingWin = 2770;

    public const int KironWin = 15500;

    public const int FirstHalfHomeTeamOddEven = 136;

    public const int OneXTwoBackLay = 9538;

    private const int MatchWinnerBackLay = 9539;

    private const int SuperOverWinnerBackLay = 9540;

    public static readonly List<int> BackLayBetType = [OneXTwoBackLay, MatchWinnerBackLay, SuperOverWinnerBackLay];
}