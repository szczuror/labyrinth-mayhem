namespace ProJob.Network.Dto;

public sealed class PlayerDto
{
    public int Row { get; set; }
    public int Col { get; set; }
    public string Symbol { get; set; } = "¶";
    public int Coins { get; set; }
    public int Gold { get; set; }
    public PlayerAttributesDto Attributes { get; set; } = new();
    public InventoryDto Inventory { get; set; } = new();
}
