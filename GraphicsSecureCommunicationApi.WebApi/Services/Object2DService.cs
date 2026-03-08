using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Services.Interfaces;

namespace GraphicsSecureCommunicationApi.WebApi.Services;

public class Object2DService : IObject2DService
{
    private readonly IObject2DRepository _repository;
    private readonly IEnvironment2DRepository _environmentRepository;

    public Object2DService(
        IObject2DRepository repository,
        IEnvironment2DRepository environmentRepository)
    {
        _repository = repository;
        _environmentRepository = environmentRepository;
    }

    public async Task<IEnumerable<Object2DDto>> GetAllByEnvironmentIdAsync(int environmentId, string userId)
    {
        return await _repository.GetAllByEnvironmentIdAsync(environmentId, userId);
    }

    public async Task<Object2DDto?> GetByIdAsync(int id, string userId)
    {
        return await _repository.GetByIdAsync(id, userId);
    }

    public async Task<Object2DDto> CreateAsync(Object2DDto obj, string userId)
    {
        await ValidateObjectAsync(obj, userId);

        return await _repository.CreateAsync(obj, userId);
    }

    public async Task<bool> UpdateAsync(Object2DDto obj, string userId)
    {
        await ValidateObjectAsync(obj, userId);

        return await _repository.UpdateAsync(obj, userId);
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        return await _repository.DeleteAsync(id, userId);
    }

    private async Task ValidateObjectAsync(Object2DDto obj, string userId)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (obj.EnvironmentId <= 0)
            throw new ArgumentException("Valid EnvironmentId is required.", nameof(obj.EnvironmentId));

        var environment = await _environmentRepository.GetByIdAsync(obj.EnvironmentId, userId);
        if (environment == null)
            throw new ArgumentException(
                $"Environment with ID {obj.EnvironmentId} not found.", 
                nameof(obj.EnvironmentId));

        if (obj.PositionX < 0 || obj.PositionX > environment.MaxLength)
            throw new ArgumentException(
                $"PositionX must be between 0 and {environment.MaxLength} (environment MaxLength).", 
                nameof(obj.PositionX));

        if (obj.PositionY < 0 || obj.PositionY > environment.MaxHeight)
            throw new ArgumentException(
                $"PositionY must be between 0 and {environment.MaxHeight} (environment MaxHeight).", 
                nameof(obj.PositionY));

        if (obj.ScaleY <= 0)
            throw new ArgumentException("ScaleY must be greater than 0.", nameof(obj.ScaleY));

        if (obj.RotationZ < 0 || obj.RotationZ >= 360)
            throw new ArgumentException("RotationZ must be between 0 and 359.", nameof(obj.RotationZ));
    }
}
