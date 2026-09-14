using ProJob.Combat;
using ProJob.Items;
using ProJob.Items.Weapons;
using ProJob.Player;

public sealed class NormalCombatVisitor : ICombatVisitor
{
    private readonly PlayerAttributes _attrs;
    public NormalCombatVisitor(PlayerAttributes attrs) => _attrs = attrs;

    public int VisitHeavy(HeavyWeapon weapon, IItem context)
        => context.Damage + _attrs.Strength + _attrs.Aggression;

    public int VisitLight(LightWeapon weapon, IItem context)
        => context.Damage + _attrs.Dexterity + _attrs.Luck;

    public int VisitMagic(MagicWeapon weapon, IItem context)
        => 1;

    public int VisitNone(IItem context)
        => 0;

    public int DefenseHeavy(HeavyWeapon weapon, IItem context) => _attrs.Strength + _attrs.Luck;
    public int DefenseLight(LightWeapon weapon, IItem context) => _attrs.Dexterity + _attrs.Luck;
    public int DefenseMagic(MagicWeapon weapon, IItem context) => _attrs.Dexterity + _attrs.Luck;
    public int DefenseNone(IItem? context) => _attrs.Dexterity;
}