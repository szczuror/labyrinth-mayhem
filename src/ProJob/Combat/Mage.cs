using ProJob.Events;

namespace ProJob.Combat;

public sealed class Mage : Enemy
{
    public Mage(DungeonEventBus? eventBus = null)
        : base("Mag", 'M', 40, 12, 2, eventBus: eventBus) { }
}
