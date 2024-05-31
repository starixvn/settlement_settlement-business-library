using Settlement.Business.Core.Models;

namespace Settlement.Business.Services;

public interface MatchService
{
    Task<Result> BlockMatch();
}

public class MatchServiceImp : MatchService
{
    public async Task<Result> BlockMatch()
    {
    }
}