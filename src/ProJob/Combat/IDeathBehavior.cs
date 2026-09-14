namespace ProJob.Combat;

public interface IDeathBehavior
{
    void OnAllyDied(Enemy survivor);
}
