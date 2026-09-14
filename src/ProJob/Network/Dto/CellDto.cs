namespace ProJob.Network.Dto;

public sealed class CellDto
{
    public int Row { get; set; }
    public int Col { get; set; }
    public List<ItemDto> Items { get; set; } = [];
}
