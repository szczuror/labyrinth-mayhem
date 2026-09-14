namespace ProJob.Input;

public sealed class KeyBindingMap
{
    private readonly IReadOnlyDictionary<GameAction, ConsoleKey[]> _map;

    public KeyBindingMap(Dictionary<GameAction, ConsoleKey[]> map)
    {
        _map = map;
    }


    public static KeyBindingMap CreateDefault() => new(new Dictionary<GameAction, ConsoleKey[]>
    {
        [GameAction.MoveUp] = [ConsoleKey.W, ConsoleKey.UpArrow],
        [GameAction.MoveDown] = [ConsoleKey.S, ConsoleKey.DownArrow],
        [GameAction.MoveLeft] = [ConsoleKey.A, ConsoleKey.LeftArrow],
        [GameAction.MoveRight] = [ConsoleKey.D, ConsoleKey.RightArrow],
        [GameAction.PickUp] = [ConsoleKey.E],
        [GameAction.ToggleInventory] = [ConsoleKey.I],
        [GameAction.UnequipLeft] = [ConsoleKey.F],
        [GameAction.UnequipRight] = [ConsoleKey.G],
        [GameAction.Quit] = [ConsoleKey.Escape],

        [GameAction.InventoryUp] = [ConsoleKey.W, ConsoleKey.UpArrow],
        [GameAction.InventoryDown] = [ConsoleKey.S, ConsoleKey.DownArrow],
        [GameAction.InventoryPageUp] = [ConsoleKey.LeftArrow],
        [GameAction.InventoryPageDown] = [ConsoleKey.RightArrow],
        [GameAction.EquipToLeft] = [ConsoleKey.L],
        [GameAction.EquipToRight] = [ConsoleKey.R],
        [GameAction.DropItem] = [ConsoleKey.D],
        [GameAction.CloseInventory] = [ConsoleKey.Q, ConsoleKey.I, ConsoleKey.Escape],

        [GameAction.MenuUp] = [ConsoleKey.W, ConsoleKey.UpArrow],
        [GameAction.MenuDown] = [ConsoleKey.S, ConsoleKey.DownArrow],
        [GameAction.MenuConfirm] = [ConsoleKey.Enter],

        [GameAction.AttackNormal] = [ConsoleKey.D1],
        [GameAction.AttackStealth] = [ConsoleKey.D2],
        [GameAction.AttackMagic] = [ConsoleKey.D3],
        [GameAction.Flee] = [ConsoleKey.Escape],

        [GameAction.ViewJournal] = [ConsoleKey.J],
    });

    public bool Matches(ConsoleKey key, GameAction action)
        => _map.TryGetValue(action, out var keys) && Array.IndexOf(keys, key) >= 0;

    public ConsoleKey[] GetKeys(GameAction action)
        => _map.TryGetValue(action, out var keys) ? keys : [];

    public string GetPrimaryLabel(GameAction action)
    {
        var keys = GetKeys(action);
        return keys.Length > 0 ? FormatKey(keys[0]) : "?";
    }

    public string GetLabel(GameAction action)
    {
        var keys = GetKeys(action);
        return keys.Length > 0 ? string.Join("/", keys.Select(FormatKey)) : "?";
    }

    public string GetCombinedLabel(params GameAction[] actions)
    {
        var keys = actions
            .SelectMany(GetKeys)
            .Distinct()
            .Select(FormatKey);
        return string.Join("/", keys);
    }

    private static string FormatKey(ConsoleKey key) => key switch
    {
        ConsoleKey.UpArrow => "↑",
        ConsoleKey.DownArrow => "↓",
        ConsoleKey.LeftArrow => "←",
        ConsoleKey.RightArrow => "→",
        ConsoleKey.Escape => "Esc",
        ConsoleKey.Enter => "Enter",
        ConsoleKey.Spacebar => "Spacja",
        ConsoleKey.Tab => "Tab",
        ConsoleKey.Backspace => "Backspace",
        _ => key.ToString(),
    };
}
