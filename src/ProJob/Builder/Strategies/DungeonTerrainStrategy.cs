namespace ProJob.Builder.Strategies;

public sealed class DungeonTerrainStrategy : IDungeonBuildStrategy
{
    public void Apply(IDungeonBuilder builder)
    {
        builder
            .WithFilledDungeon()
            .WithCorridors(250)
            .WithCentralHall()
            .WithRooms(2)
            .WithItems(25)
            .WithWeapons(555)
            .WithCurrencies(25)
            .WithEnemies(8);
    }
}