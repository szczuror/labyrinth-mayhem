using ProJob.Combat;
using ProJob.Events;
using ProJob.Items;
using ProJob.Items.Collectibles;
using ProJob.Items.Weapons;

namespace ProJob.Themes.SteampunkTheme;

public sealed class SteampunkThemeFactory : IDungeonThemeFactory
{
    private readonly Species _goblinSpecies = new("Gobliny", new CowardlyDeathBehavior());
    private readonly Species _skeletonSpecies = new("Szkielety", new AggressiveDeathBehavior());

    public string ThemeName => "Steampunk";
    public string WelcomeMessage => "Zgrzyt metalu odbija się echem od ścian...";

    public IGenerationTemplate Template { get; } = new SteampunkGenerationTemplate();

    public IReadOnlyList<Func<IItem>> ItemFactories { get; } =
    [
        () => new MetalShard(),
        () => new Gem(),
    ];

    public IReadOnlyList<Func<DungeonEventBus, Enemy>> EnemyFactories =>
    [
        bus => new CleaningRobot(bus),
        bus => new Ork(bus),
    ];

    public IReadOnlyList<SpeciesGroup> SpeciesGroups =>
    [
        new SpeciesGroup(2, bus => new Goblin(_goblinSpecies, bus)),
        new SpeciesGroup(2, bus => new Skeleton(_skeletonSpecies, bus)),
    ];

    public Func<IItem> ArtifactFactory => () => new Blaster();

    public int ItemCount => 12;
    public int EnemyCount => 3;
}
