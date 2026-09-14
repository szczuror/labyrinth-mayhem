using ProJob.Events;

namespace ProJob.Combat;

public sealed class AnimatedBriefcase : Enemy
{
    public AnimatedBriefcase(Species? species = null, DungeonEventBus? eventBus = null)
        : base("Ożywiona Teczka", 'T', 25, 6, 3, species, eventBus) { }
}
