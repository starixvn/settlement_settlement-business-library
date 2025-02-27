using Microsoft.Extensions.DependencyInjection;
using Settlement.Business.Data.Services;
using Settlement.Business.Data.Services.Products;

namespace Settlement.Business.Data;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSettlementBusinessDataServices(this IServiceCollection services)
    {
        services.AddSingleton<TicketService, TicketServiceImp>();
        services.AddSingleton<MatchService, MatchServiceImp>();
        services.AddSingleton<HistoryData, HistoryDataImp>();
        services.AddSingleton<WinLimitService, WinLimitServiceImp>();
        services.AddSingleton<SettleService, SettleServiceImp>();
        services.AddSingleton<GenerateSettleTicketService, GenerateSettleTicketServiceImp>();
        services.AddSingleton<HappyFiveProcessService, HappyFiveProcessServiceImp>();
        services.AddSingleton<RacingProcessService, RacingProcessServiceImp>();
        return services;
    }
}