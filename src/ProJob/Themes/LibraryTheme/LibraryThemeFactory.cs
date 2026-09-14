using ProJob.Combat;
using ProJob.Events;
using ProJob.Items;
using ProJob.Items.Collectibles;
using ProJob.Items.Weapons;

namespace ProJob.Themes.LibraryTheme;

public sealed class LibraryThemeFactory : IDungeonThemeFactory
{
    private readonly Species _goblinSpecies = new("Gobliny", new CowardlyDeathBehavior());
    private readonly Species _skeletonSpecies = new("Szkielety", new AggressiveDeathBehavior());

    public string ThemeName => "Biblioteka";
    public string WelcomeMessage => "Zapach starych ksiąg wypełnia powietrze...";

    public IGenerationTemplate Template { get; } = new LibraryGenerationTemplate();

    public IReadOnlyList<Func<IItem>> ItemFactories { get; } =
    [
        () => new WisdomBook(),
        () => new OldBook(),
        () => new Gem(),
    ];

    public IReadOnlyList<Func<DungeonEventBus, Enemy>> EnemyFactories =>
    [
        bus => new Mage(bus),
    ];

    public IReadOnlyList<SpeciesGroup> SpeciesGroups =>
    [
        new SpeciesGroup(2, bus => new Goblin(_goblinSpecies, bus)),
        new SpeciesGroup(20, bus => new Skeleton(_skeletonSpecies, bus)),
    ];

    public Func<IItem> ArtifactFactory => () => new BlackWand();

    public int ItemCount => 10;
    public int EnemyCount => 2;
}
