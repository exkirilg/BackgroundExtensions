using BackgroundExtensions.BackgroundServices;
using BackgroundExtensions.DbAccess;
using BackgroundExtensions.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<IDbAccess, DbAccess>();

builder.Services.AddSingleton<IExtensionValidator, ExtensionValidator>();
builder.Services.AddSingleton<IExtensionInvoker, ExtensionInvoker>();

builder.Services.AddHostedService<StoreManager>();
builder.Services.AddHostedService<ExtensionsManager>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await CreateDatabaseExtensionsTableIfNotExist(app.Services.GetRequiredService<IConfiguration>());

app.Run();

async Task CreateDatabaseExtensionsTableIfNotExist(IConfiguration config)
{
    var dbAccess = new DbAccess(config);
    await dbAccess.CreateExtensionsTable();
}
