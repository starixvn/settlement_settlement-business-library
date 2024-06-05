using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Criteria.HappyFive;
using Settlement.Business.Models;

namespace Settlement.Business.Services;

public interface MatchService
{
    Task<List<Resources>> GetResourceHappy5(int matchId, string action);

    Task<Result> LockMatch(int matchId, byte isLock, string site);
}

public class MatchServiceImp(IDynamicRepository repository) : MatchService
{
    public async Task<List<Resources>> GetResourceHappy5(int matchId, string action)
    {
        var criteria = new GetResultCriteria { MatchId = matchId, Action = action };
        return (await repository.FetchAsync<Resources>(criteria)).ToList();
    }

    public async Task<Result> LockMatch(int matchId, byte isLock, string site)
    {
        return await repository.GetAsync<Result>(new LockMatchCriteria { MatchId = matchId, Lock = isLock, DbServer = site });
    }
}