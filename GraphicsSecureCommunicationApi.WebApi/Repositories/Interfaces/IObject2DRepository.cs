using GraphicsSecureCommunicationApi.WebApi.Models.Dto;

namespace GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;

public interface IObject2DRepository
{
    Task<IEnumerable<Object2DDto>> GetAllByEnvironmentIdAsync(int environmentId, string userId);

    Task<Object2DDto?> GetByIdAsync(int id, string userId);

    Task<Object2DDto> CreateAsync(Object2DDto obj, string userId);

    Task<bool> UpdateAsync(Object2DDto obj, string userId);

    Task<bool> DeleteAsync(int id, string userId);
}
