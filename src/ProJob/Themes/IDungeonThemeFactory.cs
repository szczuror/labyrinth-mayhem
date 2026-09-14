using ProJob.Combat;
using ProJob.Events;
using ProJob.Items;

namespace ProJob.Themes;

public interface IDungeonThemeFactory
{
    string ThemeName { get; }
    string WelcomeMessage { get; }

    IGenerationTemplate Template { get; }

    IReadOnlyList<Func<IItem>> ItemFactories { get; }
    IReadOnlyList<Func<DungeonEventBus, Enemy>> EnemyFactories { get; }
    IReadOnlyList<SpeciesGroup> SpeciesGroups { get; }

    Func<IItem> ArtifactFactory { get; }

    int ItemCount { get; }
    int EnemyCount { get; }
}
