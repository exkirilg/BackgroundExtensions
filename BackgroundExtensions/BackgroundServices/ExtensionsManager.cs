using BackgroundExtensions.DbAccess;
using BackgroundExtensions.Services;

namespace BackgroundExtensions.BackgroundServices;

public class ExtensionsManager : BackgroundService
{
    private readonly int _delay;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IExtensionInvoker _invoker;

    public ExtensionsManager(IConfiguration configuration, IServiceScopeFactory scopeFactory, IExtensionInvoker invoker)
    {
        _delay = int.Parse(configuration.GetSection("Managers:ExtensionsManagerDelay").Value!);
        _scopeFactory = scopeFactory;
        _invoker = invoker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbAccess = scope.ServiceProvider.GetRequiredService<IDbAccess>();
                var extensions = await dbAccess.GetActualExtensions();
                foreach (var extension in extensions)
                {
                    _ = _invoker.TryInvokeAsync(extension);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(_delay), stoppingToken);
        }
    }
}
