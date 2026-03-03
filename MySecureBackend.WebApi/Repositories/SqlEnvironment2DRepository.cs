using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories;

public class SqlEnvironment2DRepository : IEnvironment2DRepository
{
    private readonly string _connectionString;

    public SqlEnvironment2DRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<IEnumerable<Environment2D>> GetAllByUserIdAsync(string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Environment2D WHERE UserId = @UserId";
        return await connection.QueryAsync<Environment2D>(sql, new { UserId = userId });
    }

    public async Task<Environment2D?> GetByIdAsync(int id, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Environment2D WHERE Id = @Id AND UserId = @UserId";
        return await connection.QueryFirstOrDefaultAsync<Environment2D>(sql, new { Id = id, UserId = userId });
    }

    public async Task<Environment2D> CreateAsync(Environment2D environment)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            INSERT INTO Environment2D (Name, MaxHeight, MaxLength, UserId)
            VALUES (@Name, @MaxHeight, @MaxLength, @UserId);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, environment);
        environment.Id = id;
        return environment;
    }

    public async Task<bool> UpdateAsync(Environment2D environment, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            UPDATE Environment2D 
            SET Name = @Name, MaxHeight = @MaxHeight, MaxLength = @MaxLength
            WHERE Id = @Id AND UserId = @UserId";
        
        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            environment.Id,
            environment.Name,
            environment.MaxHeight,
            environment.MaxLength,
            UserId = userId
        });
        
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "DELETE FROM Environment2D WHERE Id = @Id AND UserId = @UserId";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }
}
