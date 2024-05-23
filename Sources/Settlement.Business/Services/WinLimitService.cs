using Settlement.Business.Models;

namespace Settlement.Business.Services;

public interface WinLimitService
{
    List<WinLimit> GetMaxWinLimit(IEnumerable<Customer> customers);
}

public class WinLimitServiceImp : WinLimitService
{
    public List<WinLimit> GetMaxWinLimit(IEnumerable<Customer> customers)
    {
        var json = customers;
        return new List<WinLimit>();
    }
}