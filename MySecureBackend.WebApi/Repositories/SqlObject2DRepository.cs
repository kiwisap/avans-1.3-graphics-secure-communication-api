using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories;

public class SqlObject2DRepository : IObject2DRepository
{
    private readonly string _connectionString;

    public SqlObject2DRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<IEnumerable<Object2D>> GetAllByEnvironmentIdAsync(int environmentId, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            SELECT o.* 
            FROM Object2D o
            INNER JOIN Environment2D e ON o.EnvironmentId = e.Id
            WHERE o.EnvironmentId = @EnvironmentId AND e.UserId = @UserId";
        
        return await connection.QueryAsync<Object2D>(sql, new { EnvironmentId = environmentId, UserId = userId });
    }

    public async Task<Object2D?> GetByIdAsync(int id, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            SELECT o.* 
            FROM Object2D o
            INNER JOIN Environment2D e ON o.EnvironmentId = e.Id
            WHERE o.Id = @Id AND e.UserId = @UserId";
        
        return await connection.QueryFirstOrDefaultAsync<Object2D>(sql, new { Id = id, UserId = userId });
    }

    public async Task<Object2D> CreateAsync(Object2D obj, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var checkSql = "SELECT COUNT(1) FROM Environment2D WHERE Id = @EnvironmentId AND UserId = @UserId";
        var isAuthorized = await connection.ExecuteScalarAsync<int>(checkSql, new { obj.EnvironmentId, UserId = userId }) > 0;
        
        if (!isAuthorized)
            throw new UnauthorizedAccessException("You don't have access to this environment.");

        var sql = @"
            INSERT INTO Object2D (EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer)
            VALUES (@EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, obj);
        obj.Id = id;
        return obj;
    }

    public async Task<bool> UpdateAsync(Object2D obj, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            UPDATE o
            SET o.PrefabId = @PrefabId, 
                o.PositionX = @PositionX, 
                o.PositionY = @PositionY,
                o.ScaleX = @ScaleX,
                o.ScaleY = @ScaleY,
                o.RotationZ = @RotationZ,
                o.SortingLayer = @SortingLayer
            FROM Object2D o
            INNER JOIN Environment2D e ON o.EnvironmentId = e.Id
            WHERE o.Id = @Id AND e.UserId = @UserId";
        
        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            obj.Id,
            obj.PrefabId,
            obj.PositionX,
            obj.PositionY,
            obj.ScaleX,
            obj.ScaleY,
            obj.RotationZ,
            obj.SortingLayer,
            UserId = userId
        });
        
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            DELETE o
            FROM Object2D o
            INNER JOIN Environment2D e ON o.EnvironmentId = e.Id
            WHERE o.Id = @Id AND e.UserId = @UserId";
        
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }
}
