﻿using BackgroundExtensions.DbAccess;
using BackgroundExtensions.Models;
using BackgroundExtensions.Services;

namespace BackgroundExtensions.BackgroundServices;

public class StoreManager : BackgroundService
{
    private readonly string _directory;
    private readonly string _deprecated;
    private readonly int _delay;
    private readonly IExtensionValidator _extensionValidator;
    private readonly IServiceScopeFactory _scopeFactory;

    public StoreManager(IConfiguration configuration, IExtensionValidator extensionValidator, IServiceScopeFactory scopeFactory)
    {
        _directory = configuration.GetSection("fs:store").Value!;
        _deprecated = configuration.GetSection("fs:deprecated").Value!;
        _delay = int.Parse(configuration.GetSection("Managers:StoreManagerDelay").Value!);
        _extensionValidator = extensionValidator;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            foreach (var file in Directory.GetFiles(_directory))
            {
                FileInfo fi = new(file);

                if (fi.Name.StartsWith(_deprecated)) continue;

                if (_extensionValidator.TryValidateExtensionFile(file, out Extension extension) == false
                    || File.Exists(extension.FileName))
                {
                    File.Move(fi.FullName, Path.Combine(fi.Directory!.FullName, $"{_deprecated}{fi.Name}"));
                    continue;
                }

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbAccess = scope.ServiceProvider.GetRequiredService<IDbAccess>();
                    await dbAccess.PostExtension(extension);
                }

                File.Move(fi.FullName, extension.FileName);
            }

            await Task.Delay(TimeSpan.FromMinutes(_delay), stoppingToken);
        }
    }
}
