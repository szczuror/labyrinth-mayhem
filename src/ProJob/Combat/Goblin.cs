using ProJob.Events;

namespace ProJob.Combat;

public sealed class Goblin : Enemy
{
    public Goblin(Species species, DungeonEventBus eventBus)
        : base("Goblin", 'g', 30, 5, 2, species, eventBus) { }
}