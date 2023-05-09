using BackgroundExtensions.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace BackgroundExtensions.DbAccess;

public class DbAccess : IDbAccess
{
    private readonly string _connectionString;

    public DbAccess(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("db")!;
    }

    public async Task CreateExtensionsTable()
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(PostgreSQLScripts.CreateExtensionsTable);
    }

    public async Task PostExtension(Extension extension)
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(PostgreSQLScripts.PostExtension, extension);
    }

    public async Task<IEnumerable<Extension>> GetActualExtensions()
    {
        using IDbConnection connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Extension>(PostgreSQLScripts.GetActualExtensions);
    }
}
