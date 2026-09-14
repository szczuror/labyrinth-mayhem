using ProJob.Events;

namespace ProJob.Combat;

public sealed class Skeleton : Enemy
{
    public Skeleton(Species species, DungeonEventBus eventBus)
        : base("Szkielet", 'k', 45, 10, 4, species, eventBus) { }
}
