using ProJob.Combat;
using ProJob.Events;
using ProJob.Input;
using ProJob.Items;

namespace ProJob.Builder;

public sealed class InstructionsBuilder : IDungeonBuilder
{
    private readonly KeyBindingMap _keyMap;

    private bool _hasItems;
    private bool _hasWeapons;
    private bool _hasEnemies;

    public InstructionsBuilder(KeyBindingMap keyMap)
    {
        _keyMap = keyMap;
    }

    public IDungeonBuilder WithEmptyDungeon() => this;
    public IDungeonBuilder WithFilledDungeon() => this;
    public IDungeonBuilder WithCorridors(int count = 8) => this;
    public IDungeonBuilder WithRooms(int count = 4) => this;
    public IDungeonBuilder WithCentralHall(int width = 8, int height = 5) => this;

    public IDungeonBuilder WithItems(int count = 5)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder WithWeapons(int count = 3)
    {
        _hasWeapons = true;
        return this;
    }

    public IDungeonBuilder WithCurrencies(int count = 3)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder WithEnemies(int count = 3)
    {
        _hasEnemies = true;
        return this;
    }

    public IDungeonBuilder WithThemeItems(int count, IReadOnlyList<Func<IItem>> factories)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder WithThemeEnemies(int count, IReadOnlyList<Func<DungeonEventBus, Enemy>> factories)
    {
        _hasEnemies = true;
        return this;
    }

    public IDungeonBuilder WithSpeciesGroup(int minCount, Func<DungeonEventBus, Enemy> factory)
    {
        _hasEnemies = true;
        return this;
    }

    public IDungeonBuilder WithArtifact(Func<IItem> artifactFactory)
    {
        _hasWeapons = true;
        return this;
    }

    public IReadOnlyList<string> BuildGameModeInstructions()
    {
        var lines = new List<string>();

        string moveKeys = _keyMap.GetCombinedLabel(
            GameAction.MoveUp, GameAction.MoveDown,
            GameAction.MoveLeft, GameAction.MoveRight);
        lines.Add($"  {moveKeys} - ruch");

        if (_hasItems || _hasWeapons)
        {
            lines.Add($"  {_keyMap.GetLabel(GameAction.PickUp)} - podnieś");
            lines.Add($"  {_keyMap.GetLabel(GameAction.ToggleInventory)} - ekwipunek");
        }

        if (_hasWeapons)
        {
            lines.Add($"  {_keyMap.GetLabel(GameAction.UnequipLeft)} - zdejmij z lewej ręki");
            lines.Add($"  {_keyMap.GetLabel(GameAction.UnequipRight)} - zdejmij z prawej ręki");
        }

        if (_hasEnemies)
        {
            lines.Add($"  Podejdź do wroga - walka");
        }

        lines.Add($"  {_keyMap.GetLabel(GameAction.Quit)} - wyjdź z gry");
        lines.Add($"  {_keyMap.GetLabel(GameAction.ViewJournal)} - dziennik zdarzeń");
        return lines;
    }

    public IReadOnlyList<string> BuildInventoryModeInstructions()
    {
        string nav = _keyMap.GetCombinedLabel(GameAction.InventoryUp, GameAction.InventoryDown);
        return
        [
            $"  {nav} - nawigacja listy",
            $"  {_keyMap.GetLabel(GameAction.EquipToLeft)} - załóż w lewą rękę",
            $"  {_keyMap.GetLabel(GameAction.EquipToRight)} - załóż w prawą rękę",
            $"  {_keyMap.GetLabel(GameAction.DropItem)} - wyrzuć przedmiot",
            $"  {_keyMap.GetLabel(GameAction.CloseInventory)} - wyjdź z ekwipunku",
        ];
    }

    public IReadOnlyList<string> BuildCombatModeInstructions()
    {
        return
        [
            $"  {_keyMap.GetLabel(GameAction.AttackNormal)} - atak zwykły",
            $"  {_keyMap.GetLabel(GameAction.AttackStealth)} - atak skryty",
            $"  {_keyMap.GetLabel(GameAction.AttackMagic)} - atak magiczny",
            $"  {_keyMap.GetLabel(GameAction.Flee)} - uciekaj",
        ];
    }
}