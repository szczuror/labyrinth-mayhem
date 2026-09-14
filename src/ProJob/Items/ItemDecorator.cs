using ProJob.Combat;
using ProJob.Items;
using ProJob.Items.Weapons;
using ProJob.Network.Dto;
using ProJob.Player;

public abstract class ItemDecorator : IItem
{
    protected readonly IItem Inner;

    protected ItemDecorator(IItem inner) => Inner = inner;

    protected abstract string ModifierName { get; }

    public virtual string Name => $"{Inner.Name} ({ModifierName})";
    public virtual char Symbol => Inner.Symbol;
    public virtual string Description => Inner.Description;
    public virtual int HandsRequired => Inner.HandsRequired;
    public virtual int Damage => Inner.Damage;
    public virtual int NoiseLevel => Inner.NoiseLevel;

    public virtual string GetDetails(IItem? context = null) => Inner.GetDetails(context ?? this);

    public virtual bool OnPickedUp(Player player)
    {
        player.Inventory.AddItem(this);
        return true;
    }

    public virtual bool TryEquip(Player player, bool preferLeft)
    {
        if (HandsRequired == 0) return false;
        WeaponEquipHelper.TryEquip(player, preferLeft, this);
        return true;
    }

    public virtual void Unequip(Player player)
    {
        WeaponEquipHelper.Unequip(player, this);
    }

    public virtual bool IsCurrentlyEquipped(Player player)
        => ReferenceEquals(player.Inventory.LeftHand, this)
        || ReferenceEquals(player.Inventory.RightHand, this);

    public virtual int AcceptAttack(ICombatVisitor visitor, IItem? context = null)
        => Inner.AcceptAttack(visitor, context ?? this);

    public virtual int AcceptDefense(ICombatVisitor visitor, IItem? context = null)
        => Inner.AcceptDefense(visitor, context ?? this);

    public virtual void ApplyEquipEffect(Player player) => Inner.ApplyEquipEffect(player);
    public virtual void RemoveEquipEffect(Player player) => Inner.RemoveEquipEffect(player);

    public virtual ItemDto ToDto() => new()
    {
        Name = ModifierName,
        Symbol = Symbol.ToString(),
        Description = Description,
        HandsRequired = HandsRequired,
        Damage = Damage,
        NoiseLevel = NoiseLevel,
        Inner = Inner.ToDto(),
    };
}
