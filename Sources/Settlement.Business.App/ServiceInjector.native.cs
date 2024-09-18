using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Settlement.Business.Services;
using Settlement.Business.Services.HappyFive;

namespace Settlement.Business.App;

public static class ServiceInjector
{
    public static void InitializeServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<XmlConvertService, XmlConvertServiceImp>();
        services.AddSingleton<BatchTicketsService, BatchTicketsServiceImp>();
        services.AddSingleton<CustomerService, CustomerServiceImp>();
        services.AddSingleton<MatchService, MatchServiceImp>();
        services.AddSingleton<ModdsService, ModdsServiceImp>();

        services.AddSingleton<ModdsValueService, ModdsValueServiceImp>();
        services.AddSingleton<PrepareTicketService, PrepareTicketServiceImp>();

        services.AddSingleton<InsertHistoryData, InsertHistoryDataImp>();
        services.AddSingleton<TicketService, TicketServiceImp>();
        services.AddSingleton<WinLimitService, WinLimitServiceImp>();
        services.AddSingleton<WinLostService, WinLostServiceImp>();
        services.AddSingleton<WinLostByModdsService, WinLostByModdsServiceImp>();
        services.AddSingleton<CommissionService, CommissionServiceImp>();
        services.AddSingleton<ProcessService, ProcessServiceImp>();
        services.AddSingleton<CommandService, CommandServiceImp>();
        services.AddSingleton<Services.HappyFive.ProcessService, Services.HappyFive.ProcessServiceImp>();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });
    }
}