using ProJob.Events;

namespace ProJob.Combat;

public sealed class CleaningRobot : Enemy
{
    public CleaningRobot(DungeonEventBus? eventBus = null)
        : base("Zbuntowany Robot Sprzątający", 'R', 35, 8, 5, eventBus: eventBus) { }
}
