using ProJob.Events;

namespace ProJob.Combat;

public sealed class AnimatedSafe : Enemy
{
    public AnimatedSafe(Species? species = null, DungeonEventBus? eventBus = null)
        : base("Ożywiony Sejf", 'S', 60, 10, 8, species, eventBus) { }
}
