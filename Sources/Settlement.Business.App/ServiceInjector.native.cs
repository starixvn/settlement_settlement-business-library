using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Settlement.Business.App;

public static class ServiceInjector
{
    public static void InitializeServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<XmlConvertService, XmlConvertServiceImp>();
        services.AddSingleton<ProcessService, ProcessServiceImp>();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });
    }
}