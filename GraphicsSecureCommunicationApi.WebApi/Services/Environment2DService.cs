using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Services.Interfaces;

namespace GraphicsSecureCommunicationApi.WebApi.Services;

public class Environment2DService : IEnvironment2DService
{
    private const int MinMaxLength = 20;
    private const int MaxMaxLength = 100;
    private const int MinMaxHeight = 10;
    private const int MaxMaxHeight = 100;
    private const int MinNameLength = 1;
    private const int MaxNameLength = 25;
    private const int MaxEnvironmentsPerUser = 5;

    private readonly IEnvironment2DRepository _repository;

    public Environment2DService(IEnvironment2DRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Environment2DDto>> GetAllByUserIdAsync(string userId)
    {
        return await _repository.GetAllByUserIdAsync(userId);
    }

    public async Task<Environment2DDto?> GetByIdAsync(int id, string userId)
    {
        return await _repository.GetByIdAsync(id, userId);
    }

    public async Task<Environment2DDto> CreateAsync(Environment2DDto environment, string userId)
    {
        ValidateEnvironment(environment);

        await ValidateUniqueNameAsync(environment.Name, userId);
        await ValidateMaxEnvironmentsAsync(userId);

        return await _repository.CreateAsync(environment, userId);
    }

    public async Task<bool> UpdateAsync(Environment2DDto environment, string userId)
    {
        ValidateEnvironment(environment);
        
        await ValidateUniqueNameAsync(environment.Name, userId, environment.Id);

        return await _repository.UpdateAsync(environment, userId);
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        return await _repository.DeleteAsync(id, userId);
    }

    private void ValidateEnvironment(Environment2DDto environment)
    {
        if (environment == null)
            throw new ArgumentNullException(nameof(environment));

        if (string.IsNullOrWhiteSpace(environment.Name))
            throw new ArgumentException("Environment name is required.", nameof(environment.Name));

        if (environment.Name.Length < MinNameLength || environment.Name.Length > MaxNameLength)
            throw new ArgumentException(
                $"Environment name must be between {MinNameLength} and {MaxNameLength} characters.", 
                nameof(environment.Name));

        if (environment.MaxLength < MinMaxLength || environment.MaxLength > MaxMaxLength)
            throw new ArgumentException(
                $"MaxLength must be between {MinMaxLength} and {MaxMaxLength}.", 
                nameof(environment.MaxLength));

        if (environment.MaxHeight < MinMaxHeight || environment.MaxHeight > MaxMaxHeight)
            throw new ArgumentException(
                $"MaxHeight must be between {MinMaxHeight} and {MaxMaxHeight}.", 
                nameof(environment.MaxHeight));
    }

    private async Task ValidateUniqueNameAsync(string? name, string userId, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        var existingEnvironments = await _repository.GetAllByUserIdAsync(userId);
        var duplicate = existingEnvironments.FirstOrDefault(e => 
            e.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true && 
            e.Id != excludeId);

        if (duplicate != null)
            throw new ArgumentException(
                $"An environment with the name '{name}' already exists.", 
                nameof(name));
    }

    private async Task ValidateMaxEnvironmentsAsync(string userId)
    {
        var existingEnvironments = await _repository.GetAllByUserIdAsync(userId);
        var environmentCount = existingEnvironments.Count();

        if (environmentCount >= MaxEnvironmentsPerUser)
            throw new InvalidOperationException(
                $"You have reached the maximum limit of {MaxEnvironmentsPerUser} environments. Please delete an existing environment before creating a new one.");
    }
}
