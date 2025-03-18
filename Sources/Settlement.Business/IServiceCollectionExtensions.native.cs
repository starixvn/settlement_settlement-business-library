using Microsoft.Extensions.DependencyInjection;
using Settlement.Business.Services;

namespace Settlement.Business;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSettlementBusinessServices(this IServiceCollection services)
    {
        services.AddTransient<CommissionService, CommissionServiceImp>();
        services.AddTransient<CustomerService, CustomerServiceImp>();
        services.AddTransient<ModdsService, ModdsServiceImp>();
        services.AddTransient<ModdsValueService, ModdsValueServiceImp>();
        services.AddTransient<PrepareTicketService, PrepareTicketServiceImp>();
        services.AddTransient<TicketValueService, TicketValueServiceImp>();
        services.AddTransient<WinLostService, WinLostServiceImp>();
        services.AddTransient<WinLostByModdsService, WinLostByModdsServiceImp>();
        services.AddTransient<OutrightResultService, OutrightResultServiceImp>();

        return services;
    }
}