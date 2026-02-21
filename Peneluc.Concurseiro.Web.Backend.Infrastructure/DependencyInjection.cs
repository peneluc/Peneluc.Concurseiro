using Dapper;
using Peneluc.Concurseiro.Web.Backend.Infrastructure.Data;
using Peneluc.Concurseiro.Web.Backend.Infrastructure.Interfaces;
using Peneluc.Concurseiro.Web.Backend.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Peneluc.Concurseiro.Web.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Configuração global do Dapper
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddSingleton<IDbConnectionFactory, PostgresConnectionFactory>();

        services.AddScoped<IQuestionReadRepository, QuestionReadRepository>();

        return services;
    }
}
