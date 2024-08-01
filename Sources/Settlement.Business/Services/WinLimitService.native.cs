using Settlement.Business.Core.Constants;
using Settlement.Business.Core.Models;
using Settlement.Business.Criteria;
using System.Text.Json;

namespace Settlement.Business.Services;

public interface WinLimitService
{
    Task<IEnumerable<WinLimit>> GetMaxWinLimit(List<Ticket> tickets);
}

public class WinLimitServiceImp(
    CustomerService customerService,
    IDynamicRepository repository) : WinLimitService
{
    public async Task<IEnumerable<WinLimit>> GetMaxWinLimit(List<Ticket> tickets)
    {
        if (tickets.Count == 0 || tickets[0].SiteDBGroupId != SiteGroup.SiteDb3)
        {
            return new List<WinLimit>();
        }

        var listCustIds = customerService.GetCustomersByModds(tickets);

        var customersJson = JsonSerializer.Serialize(listCustIds);

        return await repository.FetchAsync<WinLimit>(new GetMaxPayoutSettingCriteria { ListCust = customersJson });
    }
}