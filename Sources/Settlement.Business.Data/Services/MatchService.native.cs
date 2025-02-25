using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Data.Criteria;
using Settlement.Business.Data.Criteria.HappyFive;
using Settlement.Business.Data.Criteria.Racing;

namespace Settlement.Business.Data.Services;

public interface MatchService
{
    Task<(IEnumerable<Resources>, IEnumerable<Ball>)> GetResourceHappy5(int matchId, string action);

    Task<(RaceInfo, IEnumerable<Outright>, IEnumerable<Resources>)> GetRacingInfo(int eventId);

    Task<Result> LockMatch(int matchId, byte isLock, string site, int type);
}

public class MatchServiceImp(IDynamicRepository repository) : MatchService
{
    public async Task<(IEnumerable<Resources>, IEnumerable<Ball>)> GetResourceHappy5(int matchId, string action)
    {
        var criteria = new GetResultCriteria { MatchId = matchId, Action = action };
        return await repository.FetchMultipleAsync<Resources, Ball>(criteria);
    }

    public async Task<(RaceInfo, IEnumerable<Outright>, IEnumerable<Resources>)> GetRacingInfo(int eventId)
    {
        var criteria = new GetRacingResultCriteria { EventId = eventId };
        var (firstResult, secondResult, thirdResult) = await repository.FetchMultipleAsync<RaceInfo, Outright, Resources>(criteria);
        var eventInfo = firstResult.FirstOrDefault();

        return (eventInfo, secondResult, thirdResult);
    }

    public async Task<Result> LockMatch(int matchId, byte isLock, string site, int type)
    {
        var result = new Result();
        await Policy
            .HandleResult<bool>(c => !c && isLock == 1)
            .WaitAndRetryAsync(Retry.Count,
                    _ => TimeSpan.FromSeconds(Retry.Time))
            .ExecuteAsync(async () =>
            {
                result = await repository.GetAsync<Result>(new LockMatchCriteria
                { MatchId = matchId, Lock = isLock, DbServer = site, Type = type });
                return result.IsSuccessful;
            });

        return result;
    }
}