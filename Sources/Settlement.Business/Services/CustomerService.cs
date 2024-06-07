namespace Settlement.Business.Services;

using Core.Models;
using Models;
using Settlement.Business.Core.Constants;
using System.Linq;

public interface CustomerService
{
    IEnumerable<Customer> GetCustomersByModds(IEnumerable<Ticket> tickets);
}

public class CustomerServiceImp : CustomerService
{
    public IEnumerable<Customer> GetCustomersByModds(IEnumerable<Ticket> tickets)
    {
        return (from b in tickets
                where b.Modds >= Modds.ModdsQuarter
                select new Customer
                {
                    CustId = b.CustId,
                    Currency = b.Currency
                }).DistinctBy(x => x.CustId);
    }
}