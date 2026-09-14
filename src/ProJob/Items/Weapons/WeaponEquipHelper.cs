namespace ProJob.Items.Weapons;

internal static class WeaponEquipHelper
{
    public static void TryEquip(Player.Player player, bool preferLeft, IItem item)
    {
        if (item.HandsRequired == 2 && ReferenceEquals(player.Inventory.LeftHand, item)) return;
        if (item.HandsRequired < 2)
        {
            if (preferLeft && ReferenceEquals(player.Inventory.LeftHand, item)) return;
            if (!preferLeft && ReferenceEquals(player.Inventory.RightHand, item)) return;
        }

        bool wasAlreadyEquipped = item.IsCurrentlyEquipped(player);

        if (item.HandsRequired == 2)
        {
            var left = player.Inventory.LeftHand;
            var right = player.Inventory.RightHand;

            if (left != null && !ReferenceEquals(left, item)) left.Unequip(player);
            if (right != null && !ReferenceEquals(right, left) && !ReferenceEquals(right, item)) right.Unequip(player);

            player.Inventory.SetLeftHand(item);
            player.Inventory.SetRightHand(item);
        }
        else if (preferLeft)
        {
            if (ReferenceEquals(player.Inventory.RightHand, item))
                player.Inventory.UnequipRight();

            var current = player.Inventory.LeftHand;
            if (current != null && !ReferenceEquals(current, item))
                current.Unequip(player);

            player.Inventory.SetLeftHand(item);
        }
        else
        {
            if (ReferenceEquals(player.Inventory.LeftHand, item))
                player.Inventory.UnequipLeft();

            var current = player.Inventory.RightHand;
            if (current != null && !ReferenceEquals(current, item))
                current.Unequip(player);

            player.Inventory.SetRightHand(item);
        }

        if (!wasAlreadyEquipped && item.IsCurrentlyEquipped(player))
        {
            item.ApplyEquipEffect(player);
        }
    }

    public static void Unequip(Player.Player player, IItem item)
    {
        bool wasEquipped = item.IsCurrentlyEquipped(player);

        if (item.HandsRequired == 2)
        {
            player.Inventory.UnequipLeft();
            player.Inventory.UnequipRight();
        }
        else
        {
            if (ReferenceEquals(player.Inventory.LeftHand, item))
                player.Inventory.UnequipLeft();
            else if (ReferenceEquals(player.Inventory.RightHand, item))
                player.Inventory.UnequipRight();
        }
        if (wasEquipped && !item.IsCurrentlyEquipped(player))
        {
            item.RemoveEquipEffect(player);
        }
    }
}
