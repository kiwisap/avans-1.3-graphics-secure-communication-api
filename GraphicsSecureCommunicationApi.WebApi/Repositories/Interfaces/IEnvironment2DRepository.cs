using GraphicsSecureCommunicationApi.WebApi.Models.Dto;

namespace GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;

public interface IEnvironment2DRepository
{
    Task<IEnumerable<Environment2DDto>> GetAllByUserIdAsync(string userId);

    Task<Environment2DDto?> GetByIdAsync(int id, string userId);

    Task<Environment2DDto> CreateAsync(Environment2DDto environment, string userId);

    Task<bool> UpdateAsync(Environment2DDto environment, string userId);

    Task<bool> DeleteAsync(int id, string userId);
}
