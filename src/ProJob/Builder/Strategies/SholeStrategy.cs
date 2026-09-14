namespace ProJob.Builder.Strategies;

public sealed class SholeStrategy : IDungeonBuildStrategy
{
    public void Apply(IDungeonBuilder builder)
    {
        builder
            .WithEmptyDungeon()
            .WithWeapons(1)
            .WithEnemies(3);
    }
}
