using GraphicsSecureCommunicationApi.WebApi.Data.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GraphicsSecureCommunicationApi.WebApi.Data;

public class DbContext : IDbContext
{
    private readonly string _connectionString;

    public DbContext(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
