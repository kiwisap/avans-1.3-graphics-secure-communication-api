using GraphicsSecureCommunicationApi.WebApi.Mapping.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Models.Entities;

namespace GraphicsSecureCommunicationApi.WebApi.Mapping;

public class Environment2DMappingService : IEnvironment2DMappingService
{
    public Environment2DDto Environment2DToDto(Environment2D environment2D)
    {
        return new Environment2DDto
        {
            Id = environment2D.Id,
            Name = environment2D.Name,
            MaxHeight = environment2D.MaxHeight,
            MaxLength = environment2D.MaxLength
        };
    }

    public Environment2D DtoToEnvironment2D(Environment2DDto dto, string userId)
    {
        return new Environment2D
        {
            Id = dto.Id,
            Name = dto.Name,
            MaxHeight = dto.MaxHeight,
            MaxLength = dto.MaxLength,
            UserId = userId
        };
    }
}