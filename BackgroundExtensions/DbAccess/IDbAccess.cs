using BackgroundExtensions.Models;

namespace BackgroundExtensions.DbAccess;

public interface IDbAccess
{
    Task CreateExtensionsTable();
    Task PostExtension(Extension extension);
    Task<IEnumerable<Extension>> GetActualExtensions();
}
