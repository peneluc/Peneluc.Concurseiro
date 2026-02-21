using System.Data;

namespace Peneluc.Concurseiro.Web.Backend.Infrastructure.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}
