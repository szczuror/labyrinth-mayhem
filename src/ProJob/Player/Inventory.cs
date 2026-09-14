using ProJob.Items;

namespace ProJob.Player;

public sealed class Inventory
{
    private readonly List<IItem> _items = new();

    public IReadOnlyList<IItem> Items => _items;

    public IItem? LeftHand { get; private set; }
    public IItem? RightHand { get; private set; }

    public void SetLeftHand(IItem? item) => LeftHand = item;
    public void SetRightHand(IItem? item) => RightHand = item;

    public void UnequipLeft()
    {
        if (LeftHand != null && LeftHand.HandsRequired == 2)
            RightHand = null;
        LeftHand = null;
    }

    public void UnequipRight()
    {
        if (RightHand != null && RightHand.HandsRequired == 2)
            LeftHand = null;
        RightHand = null;
    }

    public void AddItem(IItem item) => _items.Add(item);

    public IItem? RemoveItem(int index, Player player)
    {
        if (index < 0 || index >= _items.Count) return null;
        var item = _items[index];
        item.Unequip(player);
        _items.RemoveAt(index);
        return item;
    }
}
