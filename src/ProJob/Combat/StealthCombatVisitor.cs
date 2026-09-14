using ProJob.Items;
using ProJob.Items.Weapons;
using ProJob.Player;

namespace ProJob.Combat;

public sealed class StealthCombatVisitor : ICombatVisitor
{
    private readonly PlayerAttributes _attrs;

    public StealthCombatVisitor(PlayerAttributes attrs) => _attrs = attrs;

    public int VisitHeavy(HeavyWeapon weapon, IItem context)
        => (context.Damage + _attrs.Strength + _attrs.Aggression) / 2;
    public int VisitLight(LightWeapon weapon, IItem context)
        => (context.Damage + _attrs.Dexterity + _attrs.Luck) * 2;
    public int VisitMagic(MagicWeapon weapon, IItem context)
        => 1;
    public int VisitNone(IItem context)
        => 0;

    public int DefenseHeavy(HeavyWeapon weapon, IItem context) => _attrs.Strength;
    public int DefenseLight(LightWeapon weapon, IItem context) => _attrs.Dexterity;
    public int DefenseMagic(MagicWeapon weapon, IItem context) => 0;
    public int DefenseNone(IItem? context) => 0;
}
