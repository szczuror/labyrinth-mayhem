using ProJob.Builder;

namespace ProJob.Themes.VaultTheme;

public sealed class VaultGenerationTemplate : IGenerationTemplate
{
    public void Apply(IDungeonBuilder builder)
    {
        builder
            .WithFilledDungeon()
            .WithCentralHall(16, 10)
            .WithCorridors(60)
            .WithRooms(2);
    }
}
