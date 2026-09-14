using ProJob.Combat;

namespace ProJob.Items.Weapons;

public abstract class LightWeapon : Weapon
{
    public override int NoiseLevel => 1;

    public override int AcceptAttack(ICombatVisitor visitor, IItem? context = null)
        => visitor.VisitLight(this, context ?? this);
    public override int AcceptDefense(ICombatVisitor visitor, IItem? context = null)
    => visitor.DefenseLight(this, context ?? this);
}
