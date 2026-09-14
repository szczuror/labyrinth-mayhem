using ProJob.Combat;

namespace ProJob.Items.Weapons;

public abstract class MagicWeapon : Weapon
{
    public override int NoiseLevel => 3;

    public override int AcceptAttack(ICombatVisitor visitor, IItem? context = null)
    => visitor.VisitMagic(this, context ?? this);
    public override int AcceptDefense(ICombatVisitor visitor, IItem? context = null)
    => visitor.DefenseMagic(this, context ?? this);
}
