namespace ProJob.Combat;

public sealed class CowardlyDeathBehavior : IDeathBehavior
{
    public void OnAllyDied(Enemy survivor)
    {
        survivor.ModifyAttack(-2);
        survivor.ModifyArmor(-1);
    }
}
