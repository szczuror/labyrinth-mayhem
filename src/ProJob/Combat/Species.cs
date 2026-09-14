using ProJob.Observer;

namespace ProJob.Combat;

public sealed class Species : Observable<Enemy>
{
    public string Name { get; }
    public IDeathBehavior DeathBehavior { get; }

    public Species(string name, IDeathBehavior deathBehavior)
    {
        Name = name;
        DeathBehavior = deathBehavior;
    }

    public void Register(Enemy enemy) => Subscribe(enemy);

    public void NotifyDeath(Enemy deadEnemy)
    {
        Unsubscribe(deadEnemy);
        Notify(deadEnemy);
    }
}
