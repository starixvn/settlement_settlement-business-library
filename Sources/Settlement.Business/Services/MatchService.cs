using Polly;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Criteria.HappyFive;

namespace Settlement.Business.Services;

public interface MatchService
{
    Task<List<Resources>> GetResourceHappy5(int matchId, string action);

    Task<Result> LockMatch(int matchId, byte isLock, string site);
}

public class MatchServiceImp(IDynamicRepository repository) : MatchService
{
    private const byte RetryCount = 3;

    private const byte RetryTime = 3;

    public async Task<List<Resources>> GetResourceHappy5(int matchId, string action)
    {
        var criteria = new GetResultCriteria { MatchId = matchId, Action = action };
        return (await repository.FetchAsync<Resources>(criteria)).ToList();
    }

    public async Task<Result> LockMatch(int matchId, byte isLock, string site)
    {
        var result = new Result();
        await Policy
            .HandleResult<bool>(c => !c && isLock == 1)
            .WaitAndRetryAsync(RetryCount,
                    _ => TimeSpan.FromSeconds(RetryTime))
            .ExecuteAsync(async () =>
            {
                result = await repository.GetAsync<Result>(new LockMatchCriteria
                { MatchId = matchId, Lock = isLock, DbServer = site });
                return result.IsSuccessful;
            });

        return result;
    }
}