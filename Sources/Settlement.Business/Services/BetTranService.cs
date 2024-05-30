using Newtonsoft.Json;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria.HappyFive;

namespace Settlement.Business.Services;

public interface BetTranService
{
    Task<Result> Happy5UpdateBetTran(string server, List<BetTran> betTrans);
}

public class BetTranServiceImp(
    IDynamicRepository repository,
    GenerateBetTranService generateBetTransService) : BetTranService
{
    public async Task<Result> Happy5UpdateBetTran(string server, List<BetTran> betTrans)
    {
        var generateBetTrans = generateBetTransService.Happy5(betTrans);
        var betTransJson = JsonConvert.SerializeObject(generateBetTrans);
        var criteria = new HappyFiveCriteria
        {
            BetTrans = betTransJson
        };

        if (!string.IsNullOrWhiteSpace(betTransJson))
        {
            await repository.ExecuteAsync(criteria);
        }

        return new Result
        {
            ErrorCode = criteria.ErrCode,
            ErrorMessage = $"{server}: {criteria.ErrCode} - {criteria.ErrMsg}"
        };
    }
}