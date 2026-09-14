namespace ProJob.Combat;

public sealed class AggressiveDeathBehavior : IDeathBehavior
{
    public void OnAllyDied(Enemy survivor)
    {
        survivor.ModifyAttack(3);
    }
}
