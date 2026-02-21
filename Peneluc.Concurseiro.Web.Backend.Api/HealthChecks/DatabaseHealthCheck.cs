using Peneluc.Concurseiro.Web.Backend.Infrastructure.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Peneluc.Concurseiro.Web.Backend.Api.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseHealthCheck(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _connectionFactory.Create();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.ExecuteScalar();

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check failed", ex);
        }
    }
}