namespace ProJob.Combat;

using ProJob.Items;
using ProJob.Items.Weapons;

public interface ICombatVisitor
{
    int VisitHeavy(HeavyWeapon weapon, IItem context);
    int VisitLight(LightWeapon weapon, IItem context);
    int VisitMagic(MagicWeapon weapon, IItem context);
    int VisitNone(IItem context);

    int DefenseHeavy(HeavyWeapon weapon, IItem context);
    int DefenseLight(LightWeapon weapon, IItem context);
    int DefenseMagic(MagicWeapon weapon, IItem context);
    int DefenseNone(IItem? context);
}