namespace GraphicsSecureCommunicationApi.WebApi.Models;

public class Environment2D
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int MaxHeight { get; set; }
    public int MaxLength { get; set; }
    public string UserId { get; set; } = string.Empty;
}
