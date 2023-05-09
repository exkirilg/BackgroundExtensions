using BackgroundExtensions.Models;

namespace BackgroundExtensions.Services;

public interface IExtensionInvoker
{
    public Task<bool> TryInvokeAsync(Extension extension);
}
