using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using Settlement.Business.Models;
using System.Text.Json;

namespace Settlement.Business.Services;

public interface WinLimitService
{
    Task<IEnumerable<WinLimit>> GetMaxWinLimit(IEnumerable<Ticket> tickets);
}

public class WinLimitServiceImp(
    CustomerService customerService,
    IDynamicRepository repository) : WinLimitService
{
    public async Task<IEnumerable<WinLimit>> GetMaxWinLimit(IEnumerable<Ticket> tickets)
    {
        var listCustIds = customerService.GetCustomersByModds(tickets);

        var customersJson = JsonSerializer.Serialize(listCustIds);

        return await repository.FetchAsync<WinLimit>(new GetMaxPayoutSettingCriteria { ListCust = customersJson });
    }
}