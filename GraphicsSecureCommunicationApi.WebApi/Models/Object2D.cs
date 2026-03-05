namespace GraphicsSecureCommunicationApi.WebApi.Models;

public class Object2D
{
    public int Id { get; set; }
    public int EnvironmentId { get; set; }
    public int? PrefabId { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int ScaleX { get; set; }
    public int ScaleY { get; set; }
    public int RotationZ { get; set; }
    public int SortingLayer { get; set; }
}
