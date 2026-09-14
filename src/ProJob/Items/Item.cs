using ProJob.Combat;
using ProJob.Network.Dto;

namespace ProJob.Items;

public abstract class Item : IItem
{
    public abstract string Name { get; }
    public abstract char Symbol { get; }
    public abstract string Description { get; }

    public virtual bool OnPickedUp(Player.Player player)
    {
        player.Inventory.AddItem(this);
        return true;
    }
    public virtual bool TryEquip(Player.Player player, bool preferLeft)
    {
        return false;
    }

    public virtual int HandsRequired => 0;

    public virtual int Damage => 0;

    public virtual int NoiseLevel => 0;

    public virtual void Unequip(Player.Player player) { }

    public virtual bool IsCurrentlyEquipped(Player.Player player) => false;
    public virtual string GetDetails(IItem? context = null) => string.Empty;
    public virtual int AcceptAttack(ICombatVisitor visitor, IItem? context = null) => visitor.VisitNone(context ?? this);
    public virtual int AcceptDefense(ICombatVisitor visitor, IItem? context = null) => visitor.DefenseNone(context ?? this);
    public virtual void ApplyEquipEffect(Player.Player player) { }
    public virtual void RemoveEquipEffect(Player.Player player) { }

    public override string ToString() => Name;

    public virtual ItemDto ToDto() => new()
    {
        Name = Name,
        Symbol = Symbol.ToString(),
        Description = Description,
        HandsRequired = HandsRequired,
        Damage = Damage,
        NoiseLevel = NoiseLevel,
    };
}
