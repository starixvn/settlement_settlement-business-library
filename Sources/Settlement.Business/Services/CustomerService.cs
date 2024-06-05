namespace Settlement.Business.Services;

using Core.Models;
using Models;
using Settlement.Business.Core.Constants;
using System.Linq;

public interface CustomerService
{
    IEnumerable<Customer> GetCustomersByModds(IEnumerable<BetTran> betTrans);
}

public class CustomerServiceImp : CustomerService
{
    public IEnumerable<Customer> GetCustomersByModds(IEnumerable<BetTran> betTrans)
    {
        return (from b in betTrans
                where b.Modds >= Modds.ModdsQuarter
                select new Customer
                {
                    CustId = b.CustId,
                    Currency = b.Currency
                }).DistinctBy(x => x.CustId).ToList();
    }
}