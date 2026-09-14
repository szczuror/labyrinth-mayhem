using ProJob.Events;

namespace ProJob.Combat;

public sealed class Ork : Enemy
{
    public Ork(DungeonEventBus? eventBus = null)
        : base("Ork", 'O', 60, 15, 5, eventBus: eventBus) { }
}