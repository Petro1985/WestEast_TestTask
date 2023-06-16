using System.Net;
using DAL;
using DAL.Repository;
using Infrastructure.HttpClients;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Services.Interfaces;
using Services.Parsers;
using Services.Services;
using WestEast_TestTask;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddDbContextFactory<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("OwnDb")));

builder.Services.AddSingleton<WebApiLoader>();
builder.Services.Scan(x =>
    x.FromAssemblyOf<IOrderParser>()
        .AddClasses(cl => cl.AssignableTo<IOrderParser>())
        .As<IOrderParser>()
        .WithSingletonLifetime());

builder.Services.AddSingleton<ILoadHistoryRepository, LoadHistoryRepository>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<WebApiLoader>();

var errorPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryForeverAsync(
    (tryCount, context) => TimeSpan.FromSeconds(5 * Math.Max(tryCount, 10)));

var httpClientUri = builder.Configuration["ClientUri"] ?? "";
builder.Services.AddHttpClient<IClientApiGateway, ClientApiGatewayApiGateway>(x =>
{
    x.BaseAddress = new Uri(httpClientUri);
}).AddPolicyHandler(errorPolicy);

builder.Services.AddHostedService<HostedService<WebApiLoader>>();

var app = builder.Build();

app.Run();