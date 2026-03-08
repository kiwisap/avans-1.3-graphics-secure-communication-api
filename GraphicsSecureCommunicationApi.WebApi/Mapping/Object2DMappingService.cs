using GraphicsSecureCommunicationApi.WebApi.Mapping.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Models.Entities;

namespace GraphicsSecureCommunicationApi.WebApi.Mapping;

public class Object2DMappingService : IObject2DMappingService
{
    public Object2DDto Object2DToDto(Object2D object2D)
    {
        return new Object2DDto
        {
            Id = object2D.Id,
            EnvironmentId = object2D.EnvironmentId,
            PrefabId = object2D.PrefabId,
            PositionX = object2D.PositionX,
            PositionY = object2D.PositionY,
            ScaleX = object2D.ScaleX,
            ScaleY = object2D.ScaleY,
            RotationZ = object2D.RotationZ,
            SortingLayer = object2D.SortingLayer
        };
    }

    public Object2D DtoToObject2D(Object2DDto dto)
    {
        return new Object2D
        {
            Id = dto.Id,
            EnvironmentId = dto.EnvironmentId,
            PrefabId = dto.PrefabId,
            PositionX = dto.PositionX,
            PositionY = dto.PositionY,
            ScaleX = dto.ScaleX,
            ScaleY = dto.ScaleY,
            RotationZ = dto.RotationZ,
            SortingLayer = dto.SortingLayer
        };
    }
}
