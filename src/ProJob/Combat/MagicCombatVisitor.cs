using ProJob.Items;
using ProJob.Items.Weapons;
using ProJob.Player;

namespace ProJob.Combat;

public sealed class MagicCombatVisitor : ICombatVisitor
{
    private readonly PlayerAttributes _attrs;

    public MagicCombatVisitor(PlayerAttributes attrs) => _attrs = attrs;

    public int VisitHeavy(HeavyWeapon weapon, IItem context)
        => 1;
    public int VisitLight(LightWeapon weapon, IItem context)
        => 1;
    public int VisitMagic(MagicWeapon weapon, IItem context)
        => context.Damage + _attrs.Wisdom;
    public int VisitNone(IItem context)
        => 0;

    public int DefenseHeavy(HeavyWeapon weapon, IItem context) => _attrs.Luck;
    public int DefenseLight(LightWeapon weapon, IItem context) => _attrs.Luck;
    public int DefenseMagic(MagicWeapon weapon, IItem context) => _attrs.Wisdom * 2;
    public int DefenseNone(IItem? context) => _attrs.Luck;
}
