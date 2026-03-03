using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories;

public interface IEnvironment2DRepository
{
    Task<IEnumerable<Environment2D>> GetAllByUserIdAsync(string userId);

    Task<Environment2D?> GetByIdAsync(int id, string userId);

    Task<Environment2D> CreateAsync(Environment2D environment);

    Task<bool> UpdateAsync(Environment2D environment, string userId);

    Task<bool> DeleteAsync(int id, string userId);
}
