using ProJob.Combat;
using ProJob.Events;
using ProJob.Items;
using ProJob.Items.Currencies;
using ProJob.Items.Weapons;

namespace ProJob.Themes.VaultTheme;

public sealed class VaultThemeFactory : IDungeonThemeFactory
{
    private readonly Species _briefcaseSpecies = new("Teczki", new AggressiveDeathBehavior());
    private readonly Species _safeSpecies = new("Sejfy", new CowardlyDeathBehavior());

    public string ThemeName => "Skarbiec";
    public string WelcomeMessage => "Czujesz swędzenie w portfelu...";

    public IGenerationTemplate Template { get; } = new VaultGenerationTemplate();

    public IReadOnlyList<Func<IItem>> ItemFactories { get; } =
    [
        () => new Coin(),
        () => new Gold(),
    ];

    public IReadOnlyList<Func<DungeonEventBus, Enemy>> EnemyFactories => [];

    public IReadOnlyList<SpeciesGroup> SpeciesGroups =>
    [
        new SpeciesGroup(2, bus => new AnimatedBriefcase(_briefcaseSpecies, bus)),
        new SpeciesGroup(2, bus => new AnimatedSafe(_safeSpecies, bus)),
    ];

    public Func<IItem> ArtifactFactory => () => new LuckyCoinSatchel();

    public int ItemCount => 15;
    public int EnemyCount => 0;
}
