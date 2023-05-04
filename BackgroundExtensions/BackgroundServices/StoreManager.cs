using BackgroundExtensions.Models;
using BackgroundExtensions.Services;

namespace BackgroundExtensions.BackgroundServices;

public class StoreManager : BackgroundService
{
    private readonly string _storeDirectory;
    private readonly IExtensionValidator _extensionValidator;

    public StoreManager(IConfiguration configuration, IExtensionValidator extensionValidator)
    {
        _storeDirectory = configuration.GetSection("fs:store").Value!;
        _extensionValidator = extensionValidator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            foreach (var file in Directory.GetFiles(_storeDirectory))
            {
                Extension extension;
                if (_extensionValidator.TryValidateExtensionFile(file, out extension) == false)
                    continue;
                

            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
