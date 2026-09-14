using ProJob.Combat;
using ProJob.Network.Dto;

namespace ProJob.Items;

public interface IItem
{
    string Name { get; }
    char Symbol { get; }
    string Description { get; }
    int HandsRequired { get; }
    int Damage { get; }
    int NoiseLevel { get; }

    ItemDto ToDto();

    bool OnPickedUp(Player.Player player);
    bool TryEquip(Player.Player player, bool preferLeft);
    void Unequip(Player.Player player);
    bool IsCurrentlyEquipped(Player.Player player);

    string GetDetails(IItem? context = null);
    int AcceptAttack(ICombatVisitor visitor, IItem? context = null);
    int AcceptDefense(ICombatVisitor visitor, IItem? context = null);
    void ApplyEquipEffect(Player.Player player);
    void RemoveEquipEffect(Player.Player player);
}