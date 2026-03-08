using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Models.Entities;

namespace GraphicsSecureCommunicationApi.WebApi.Mapping.Interfaces;

public interface IEnvironment2DMappingService
{
    Environment2DDto Environment2DToDto(Environment2D environment2D);

    Environment2D DtoToEnvironment2D(Environment2DDto dto, string userId);
}