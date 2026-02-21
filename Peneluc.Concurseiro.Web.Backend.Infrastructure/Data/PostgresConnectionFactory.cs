using Peneluc.Concurseiro.Web.Backend.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace Peneluc.Concurseiro.Web.Backend.Infrastructure.Data;

public class PostgresConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public PostgresConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ConcurseiroDatabase")
                            ?? configuration.GetConnectionString("TelemetriaDb")
                            ?? configuration.GetConnectionString("TelemetriaDb1")
                            ?? throw new InvalidOperationException("No Concurseiro connection string found. Expected 'ConcurseiroDatabase' or 'TelemetriaDb' or 'TelemetriaDb1'.");
    }

    public DbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var conn = CreateConnection();
        await conn.OpenAsync(cancellationToken);
        return conn;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
