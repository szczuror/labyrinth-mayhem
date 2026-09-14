using ProJob.Builder;

namespace ProJob.Themes.SteampunkTheme;

public sealed class SteampunkGenerationTemplate : IGenerationTemplate
{
    public void Apply(IDungeonBuilder builder)
    {
        builder
            .WithFilledDungeon()
            .WithRooms(20)
            .WithCorridors(80)
            .WithCentralHall(10, 6);
    }
}
