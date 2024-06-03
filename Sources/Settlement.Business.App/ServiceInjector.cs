using Fanex.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Settlement.Business.Services;

namespace Settlement.Business.App;

public static class ServiceInjector
{
    public static void InitializeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<BatchTicketsService, BatchTicketsServiceImp>();
        services.AddSingleton<CommandService, CommandServiceImp>();
        services.AddSingleton<CustomerService, CustomerServiceImp>();
        services.AddSingleton<GenerateBetTranService, GenerateBetTranServiceImp>();
        services.AddSingleton<MatchService, MatchServiceImp>();
        services.AddSingleton<ModdsService, ModdsServiceImp>();

        services.AddSingleton<ModdsValueService, ModdsValueServiceImp>();
        services.AddSingleton<PrepareBetTranService, PrepareBetTranServiceImp>();

        services.AddSingleton<TicketService, TicketServiceImp>();
        services.AddSingleton<WinLimitService, WinLimitServiceImp>();
        services.AddSingleton<WinLostService, WinLostServiceImp>();
        services.AddSingleton<ProcessService, ProcessServiceImp>();
    }
}