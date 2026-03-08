using Dapper;
using Microsoft.Data.SqlClient;
using GraphicsSecureCommunicationApi.WebApi.Models.Entities;
using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Mapping.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;

namespace GraphicsSecureCommunicationApi.WebApi.Repositories;

public class SqlObject2DRepository : IObject2DRepository
{
    private readonly string _connectionString;
    private readonly IObject2DMappingService _object2DMappingService;

    public SqlObject2DRepository(
        string connectionString,
        IObject2DMappingService object2DMappingService)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _object2DMappingService = object2DMappingService;
    }

    public async Task<IEnumerable<Object2DDto>> GetAllByEnvironmentIdAsync(int environmentId, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            SELECT o.* 
            FROM Object2D o
            INNER JOIN Environment2D e ON o.EnvironmentId = e.Id
            WHERE o.EnvironmentId = @EnvironmentId AND e.UserId = @UserId";

        var entities = await connection.QueryAsync<Object2D>(sql, new { EnvironmentId = environmentId, UserId = userId });
        return [.. entities.Select(_object2DMappingService.Object2DToDto)];
    }

    public async Task<Object2DDto?> GetByIdAsync(int id, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            SELECT o.* 
            FROM Object2D o
            INNER JOIN Environment2D e ON o.EnvironmentId = e.Id
            WHERE o.Id = @Id AND e.UserId = @UserId";

        var entity = await connection.QueryFirstOrDefaultAsync<Object2D>(sql, new { Id = id, UserId = userId });
        return entity != null ? _object2DMappingService.Object2DToDto(entity) : null;
    }

    public async Task<Object2DDto> CreateAsync(Object2DDto obj, string userId)
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

        var entity = _object2DMappingService.DtoToObject2D(obj);
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        obj.Id = id;
        return obj;
    }

    public async Task<bool> UpdateAsync(Object2DDto obj, string userId)
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

        var entity = _object2DMappingService.DtoToObject2D(obj);
        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            entity.Id,
            entity.PrefabId,
            entity.PositionX,
            entity.PositionY,
            entity.ScaleX,
            entity.ScaleY,
            entity.RotationZ,
            entity.SortingLayer,
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
