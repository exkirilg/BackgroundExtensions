using BackgroundExtensions.Models;

namespace BackgroundExtensions.Services
{
    public interface IExtensionValidator
    {
        bool TryValidateExtensionFile(string file, out Extension extension);
    }
}
