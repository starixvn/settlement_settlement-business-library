using Polly;
using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Criteria.HappyFive;

namespace Settlement.Business.Services;

public interface MatchService
{
    Task<(IEnumerable<Resources>, IEnumerable<Ball>)> GetResourceHappy5(int matchId, string action);

    Task<Result> LockMatch(int matchId, byte isLock, string site);
}

public class MatchServiceImp(IDynamicRepository repository) : MatchService
{
    public async Task<(IEnumerable<Resources>, IEnumerable<Ball>)> GetResourceHappy5(int matchId, string action)
    {
        var criteria = new GetResultCriteria { MatchId = matchId, Action = action };
        return await repository.FetchMultipleAsync<Resources, Ball>(criteria);
    }

    public async Task<Result> LockMatch(int matchId, byte isLock, string site)
    {
        var result = new Result();
        await Policy
            .HandleResult<bool>(c => !c && isLock == 1)
            .WaitAndRetryAsync(Retry.Count,
                    _ => TimeSpan.FromSeconds(Retry.Time))
            .ExecuteAsync(async () =>
            {
                result = await repository.GetAsync<Result>(new LockMatchCriteria
                { MatchId = matchId, Lock = isLock, DbServer = site });
                return result.IsSuccessful;
            });

        return result;
    }
}