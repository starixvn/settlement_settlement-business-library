using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Models;
using System.Text.Json;

namespace Settlement.Business.Services;

public interface WinLimitService
{
    Task<IEnumerable<WinLimit>> GetMaxWinLimit(IEnumerable<BetTran> betTrans);
}

public class WinLimitServiceImp(
    CustomerService customerService,
    IDynamicRepository repository) : WinLimitService
{
    public async Task<IEnumerable<WinLimit>> GetMaxWinLimit(IEnumerable<BetTran> betTrans)
    {
        var listCustIds = customerService.GetCustomersByModds(betTrans);

        var customersJson = JsonSerializer.Serialize(listCustIds);

        return await repository.FetchAsync<WinLimit>(new GetMaxPayoutSettingCriteria { ListCust = customersJson });
    }
}