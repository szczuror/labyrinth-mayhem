using ProJob.Events;

namespace ProJob.Combat;

public sealed class SpeciesGroup
{
    public int MinCount { get; }
    public Func<DungeonEventBus, Enemy> Factory { get; }

    public SpeciesGroup(int minCount, Func<DungeonEventBus, Enemy> factory)
    {
        MinCount = minCount;
        Factory = factory;
    }
}
