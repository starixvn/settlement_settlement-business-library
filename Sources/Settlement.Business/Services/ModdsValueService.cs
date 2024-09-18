using Settlement.Business.Constants;

namespace Settlement.Business.Services;

public interface ModdsValueService
{
    int GetValueByProfileIdOfHappy5(int profileId, string homeValue, string awayValue);
}

public class ModdsValueServiceImp : ModdsValueService
{
    public int GetValueByProfileIdOfHappy5(int profileId, string homeValue, string awayValue)
    {
        return profileId switch
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