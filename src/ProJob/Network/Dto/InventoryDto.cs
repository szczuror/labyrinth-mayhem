namespace ProJob.Network.Dto;

public sealed class InventoryDto
{
    public List<ItemDto> Items { get; set; } = [];
    public ItemDto? LeftHand { get; set; }
    public ItemDto? RightHand { get; set; }
}
