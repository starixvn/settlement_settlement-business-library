using Settlement.Business.Constants;

namespace Settlement.Business.Services;

public interface ModdsService
{
    int Happy5(int profileId, string betTeam, string homeValue, string awayValue);
}

public class ModdsServiceImp(ModdsValueService moddsValueService) : ModdsService
{
    public int Happy5(int profileId, string betTeam, string homeValue, string awayValue)
    {
        var mainValue = moddsValueService.GetValueByProfileIdOfHappy5(profileId, homeValue, awayValue);

        if (HappyFiveConstants.Evaluate.TryGetValue((profileId, betTeam), out var evaluate))
        {
            return evaluate(mainValue) ? 1 : -1;
        }

        return -1;
    }
}