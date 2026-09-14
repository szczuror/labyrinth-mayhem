using ProJob.Combat;

namespace ProJob.Items.Weapons;

public abstract class HeavyWeapon : Weapon
{
    public override int NoiseLevel => 5;

    public override int AcceptAttack(ICombatVisitor visitor, IItem? context = null)
    => visitor.VisitHeavy(this, context ?? this);
    public override int AcceptDefense(ICombatVisitor visitor, IItem? context = null)
    => visitor.DefenseHeavy(this, context ?? this);
}
