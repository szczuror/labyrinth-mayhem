using ProJob.Builder;

namespace ProJob.Themes.LibraryTheme;

public sealed class LibraryGenerationTemplate : IGenerationTemplate
{
    public void Apply(IDungeonBuilder builder)
    {
        builder
            .WithFilledDungeon()
            .WithCorridors(200)
            .WithCentralHall(6, 4)
            .WithRooms(3)
            .WithWeapons(10);
    }
}
