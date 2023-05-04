using BackgroundExtensions.DbAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<IDbAccess, DbAccess>();

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
