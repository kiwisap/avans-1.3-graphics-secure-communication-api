using System.Data;

namespace GraphicsSecureCommunicationApi.WebApi.Data.Interfaces;

public interface IDbContext
{
    IDbConnection CreateConnection();
}
