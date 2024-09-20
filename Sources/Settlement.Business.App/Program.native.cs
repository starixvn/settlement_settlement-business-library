using Fanex.Data.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Settlement.Business.App;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFanexSql(builder.Configuration);

builder.Services.InitializeServices();

builder.Services.AddHostedService<TestService>();

var app = builder.Build();

app.Run(async context =>
{
    await context.Response.WriteAsync("OK");
});

app.Run();