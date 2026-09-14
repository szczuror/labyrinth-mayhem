using ProJob.Combat;
using ProJob.Events;
using ProJob.Items;

namespace ProJob.Builder;

public interface IDungeonBuilder
{
    IDungeonBuilder WithEmptyDungeon();
    IDungeonBuilder WithFilledDungeon();
    IDungeonBuilder WithCorridors(int count = 8);
    IDungeonBuilder WithRooms(int count = 4);
    IDungeonBuilder WithCentralHall(int width = 8, int height = 5);
    IDungeonBuilder WithItems(int count = 5);
    IDungeonBuilder WithWeapons(int count = 3);
    IDungeonBuilder WithCurrencies(int count = 2);
    IDungeonBuilder WithEnemies(int count = 3);
    IDungeonBuilder WithThemeItems(int count, IReadOnlyList<Func<IItem>> factories);
    IDungeonBuilder WithThemeEnemies(int count, IReadOnlyList<Func<DungeonEventBus, Enemy>> factories);
    IDungeonBuilder WithSpeciesGroup(int minCount, Func<DungeonEventBus, Enemy> factory);
    IDungeonBuilder WithArtifact(Func<IItem> artifactFactory);
}