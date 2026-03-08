using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Models.Entities;

namespace GraphicsSecureCommunicationApi.WebApi.Mapping.Interfaces;

public interface IObject2DMappingService
{
    Object2DDto Object2DToDto(Object2D object2D);

    Object2D DtoToObject2D(Object2DDto dto);
}