using BackgroundExtensions.Models;
using System.Reflection;

namespace BackgroundExtensions.Services;

public class ExtensionInvoker : IExtensionInvoker
{
    private static readonly List<string> _activeExtensions = new();

    private readonly IConfiguration _configuration;

    public ExtensionInvoker(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> TryInvokeAsync(Extension extension)
    {
        if (IsActive(extension)) return false;

        _activeExtensions.Add(extension.Name);

        try
        {
            await InvokeAsync(extension);
        }
        catch
        {
            return false;
        }
        finally
        {
            _activeExtensions.Remove(extension.Name);
        }

        return true;
    }

    private bool IsActive(Extension extension)
    {
        return _activeExtensions.Contains(extension.Name);
    }

    private async Task InvokeAsync(Extension extension)
    {
        Assembly assembly = Assembly.LoadFrom(extension.FileName);

        var type = assembly.GetType(_configuration.GetSection("Extension:Class").Value!)!;
        var instance = Activator.CreateInstance(type);
        var method = type.GetMethod(_configuration.GetSection("Extension:Method").Value!);
        Task result = (Task)method!.Invoke(instance, new object[] { })!;
        await result;
    }
}
