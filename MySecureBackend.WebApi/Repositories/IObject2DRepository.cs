using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories;

public interface IObject2DRepository
{
    Task<IEnumerable<Object2D>> GetAllByEnvironmentIdAsync(int environmentId, string userId);
    Task<Object2D?> GetByIdAsync(int id, string userId);
    Task<Object2D> CreateAsync(Object2D obj, string userId);
    Task<bool> UpdateAsync(Object2D obj, string userId);
    Task<bool> DeleteAsync(int id, string userId);
}
