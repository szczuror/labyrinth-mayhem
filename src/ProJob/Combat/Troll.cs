using ProJob.Events;

namespace ProJob.Combat;

public sealed class Troll : Enemy
{
    public Troll(DungeonEventBus? eventBus = null)
        : base("Troll", 't', 100, 18, 10, eventBus: eventBus) { }

    public override int PerformAttack()
    {
        var random = new Random();
        int variation = random.Next(-2, 3);
        return EffectiveAttack + variation;
    }
}