namespace ProJob.Items.Weapons;

public abstract class Weapon : Item
{
    public override string GetDetails(IItem? context = null)
    {
        var actualItem = context ?? this;
        return $"DMG: {actualItem.Damage}";
    }

    public override void Unequip(Player.Player player)
    {
        WeaponEquipHelper.Unequip(player, this);
    }

    public override bool TryEquip(Player.Player player, bool preferLeft)
    {
        WeaponEquipHelper.TryEquip(player, preferLeft, this);
        return true;
    }

    public override bool IsCurrentlyEquipped(Player.Player player)
        => ReferenceEquals(player.Inventory.LeftHand, this)
        || ReferenceEquals(player.Inventory.RightHand, this);
}
