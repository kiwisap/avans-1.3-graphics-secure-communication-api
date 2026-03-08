using Dapper;
using GraphicsSecureCommunicationApi.WebApi.Mapping.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Models.Entities;
using GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace GraphicsSecureCommunicationApi.WebApi.Repositories;

public class SqlEnvironment2DRepository : IEnvironment2DRepository
{
    private readonly string _connectionString;

    private readonly IEnvironment2DMappingService _environment2DMappingService;

    public SqlEnvironment2DRepository(
        string connectionString,
        IEnvironment2DMappingService environment2DMappingService)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        _environment2DMappingService = environment2DMappingService;
    }

    public async Task<IEnumerable<Environment2DDto>> GetAllByUserIdAsync(string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Environment2D WHERE UserId = @UserId";
        
        var entities = await connection.QueryAsync<Environment2D>(sql, new { UserId = userId });
        return [.. entities.Select(_environment2DMappingService.Environment2DToDto)];
    }

    public async Task<Environment2DDto?> GetByIdAsync(int id, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Environment2D WHERE Id = @Id AND UserId = @UserId";
        
        var entity = await connection.QueryFirstOrDefaultAsync<Environment2D>(sql, new { Id = id, UserId = userId });
        return entity != null ? _environment2DMappingService.Environment2DToDto(entity) : null;
    }

    public async Task<Environment2DDto> CreateAsync(Environment2DDto environment, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            INSERT INTO Environment2D (Name, MaxHeight, MaxLength, UserId)
            VALUES (@Name, @MaxHeight, @MaxLength, @UserId);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        var entity = _environment2DMappingService.DtoToEnvironment2D(environment, userId);
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        environment.Id = id;
        return environment;
    }

    public async Task<bool> UpdateAsync(Environment2DDto environment, string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            UPDATE Environment2D 
            SET Name = @Name, MaxHeight = @MaxHeight, MaxLength = @MaxLength
            WHERE Id = @Id AND UserId = @UserId";

        var entity = _environment2DMappingService.DtoToEnvironment2D(environment, userId);
        var rowsAffected = await connection.ExecuteAsync(sql, entity);

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
