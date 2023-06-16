using Microsoft.Extensions.Hosting;
using Services.Services;

namespace WestEast_TestTask;

public class HostedService<TLoader> : BackgroundService where TLoader : LoaderBase
{
    private readonly TLoader _loader;

    public HostedService(TLoader loader)
    {
        _loader = loader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) await _loader.Load();
    }
}