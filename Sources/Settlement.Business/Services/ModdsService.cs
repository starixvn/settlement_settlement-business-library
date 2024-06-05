using Settlement.Business.Constants;
using Settlement.Business.Models;

namespace Settlement.Business.Services;

public interface ModdsService
{
    int Happy5(int profileId, string betTeam, List<Resources> resources);
}

public class ModdsServiceImp(ModdsValueService moddsValueService) : ModdsService
{
    public int Happy5(int profileId, string betTeam, List<Resources> resources)
    {
        var resource = resources.Find(r => r.ProfileId == profileId) ?? new Resources();
        var mainValue = moddsValueService.GetValueByProfileIdOfHappy5(profileId, resource.Result1, resource.Result2);

        if (HappyFiveConstants.Evaluate.TryGetValue((profileId, betTeam), out var evaluate))
        {
            return evaluate(mainValue) ? 1 : -1;
        }

        return -1;
    }
}