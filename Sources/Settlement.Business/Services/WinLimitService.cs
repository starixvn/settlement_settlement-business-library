using Newtonsoft.Json;
using Settlement.Business.Core.Models;
using Settlement.Business.Models;

namespace Settlement.Business.Services;

public interface WinLimitService
{
    List<WinLimit> GetMaxWinLimit(IEnumerable<BetTran> betTrans);
}

public class WinLimitServiceImp(CustomerService customerService) : WinLimitService
{
    public List<WinLimit> GetMaxWinLimit(IEnumerable<BetTran> betTrans)
    {
        var listCustIds = customerService.GetCustomersByModds(betTrans);

        var json = JsonConvert.SerializeObject(listCustIds);
        return new List<WinLimit>();
    }
}