namespace ProJob.Builder.Strategies;

public sealed class RoomStrategy : IDungeonBuildStrategy
{
    public void Apply(IDungeonBuilder builder)
    {
        builder
            .WithFilledDungeon()
            .WithCentralHall()
            .WithRooms(25)
            .WithEnemies(5);
    }
}
