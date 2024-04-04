// Ignore Spelling: Modds

using settlement_business_library.Constants;

namespace settlement_business_library;

public static class HappyFiveModds
{
    public static int GetModds(int profileID, string betTeam, string homeValue, string awayValue)
    {
        var mainValue = GetMainValue(profileID, homeValue, awayValue);

        if (HappyFiveConstants.Evaluate.TryGetValue((profileID, betTeam), out var evaluate))
        {
            return evaluate(mainValue) ? 1 : -1;
        }

        return -1;
    }


    private static int GetMainValue(int profileID, string homeValue, string awayValue)
    {
        return profileID switch
        {
            HappyFiveConstants.FirstProfileId or
            HappyFiveConstants.ThirdProfileId or
            HappyFiveConstants.FifthProfileId => int.Parse(homeValue),
            HappyFiveConstants.SecondProfileId => int.Parse(homeValue) % 2,
            HappyFiveConstants.FourthProfileId => int.Parse(homeValue) - int.Parse(awayValue),
            _ => -1
        };
    }
}
