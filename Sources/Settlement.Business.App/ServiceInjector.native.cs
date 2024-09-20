using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Settlement.Business.Services;

namespace Settlement.Business.App;

public static class ServiceInjector
{
    public static void InitializeServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<XmlConvertService, XmlConvertServiceImp>();
        services.AddSingleton<CustomerService, CustomerServiceImp>();
        services.AddSingleton<MatchService, MatchServiceImp>();
        services.AddSingleton<ModdsService, ModdsServiceImp>();

        services.AddSingleton<ModdsValueService, ModdsValueServiceImp>();
        services.AddSingleton<PrepareTicketService, PrepareTicketServiceImp>();

        services.AddSingleton<HistoryData, HistoryDataImp>();
        services.AddSingleton<TicketService, TicketServiceImp>();
        services.AddSingleton<WinLimitService, WinLimitServiceImp>();
        services.AddSingleton<WinLostService, WinLostServiceImp>();
        services.AddSingleton<WinLostByModdsService, WinLostByModdsServiceImp>();
        services.AddSingleton<CommissionService, CommissionServiceImp>();
        services.AddSingleton<ProcessService, ProcessServiceImp>();

        services.AddSingleton<SettleService, SettleServiceImp>();
        services.AddSingleton<TicketValueService, TicketValueServiceImp>();

        services.AddSingleton<Services.HappyFive.BatchTicketsService, Services.HappyFive.BatchTicketsServiceImp>();
        services.AddSingleton<Services.HappyFive.ProcessService, Services.HappyFive.ProcessServiceImp>();

        services.AddSingleton<Services.Racing.BatchTicketsService, Services.Racing.BatchTicketsServiceImp>();
        services.AddSingleton<Services.Racing.ProcessService, Services.Racing.ProcessServiceImp>();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });
    }
}