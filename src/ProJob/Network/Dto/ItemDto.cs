namespace ProJob.Network.Dto;

public sealed class ItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HandsRequired { get; set; }
    public int Damage { get; set; }
    public int NoiseLevel { get; set; }
    public ItemDto? Inner { get; set; }
}
