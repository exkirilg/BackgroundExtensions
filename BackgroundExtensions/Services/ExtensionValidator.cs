using BackgroundExtensions.Models;
using System.Diagnostics;
using System.Reflection;

namespace BackgroundExtensions.Services;

public class ExtensionValidator : IExtensionValidator
{
    private readonly IConfiguration _configuration;

    public ExtensionValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool TryValidateExtensionFile(string file, out Extension extension)
    {
        extension = new();

        Assembly? assembly = null;

        try
        {
            assembly = Assembly.LoadFrom(file);
        }
        catch { }

        if (assembly is null)
            return false;

        string? name = assembly.GetName().Name;
        if (name is null)
            return false;
        extension.Name = name;

        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        string version = fvi.FileVersion ?? "1.0.0.0";
        extension.Version = version;

        extension.FileName = Path.Combine(_configuration.GetSection("fs:active").Value!, $"{name}_{version}.dll");

        Type? type = assembly.GetType(_configuration.GetSection("Extension:Class").Value!);

        if (type is null)
            return false;

        MethodInfo? method = type.GetMethod(_configuration.GetSection("Extension:Method").Value!);
        if (method is null)
            return false;

        return true;
    }
}
