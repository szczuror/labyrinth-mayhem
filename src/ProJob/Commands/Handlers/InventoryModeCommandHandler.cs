using ProJob.Core;
using ProJob.Input;

namespace ProJob.Commands.Handlers;

public sealed class InventoryModeCommandHandler : CommandHandler
{
    private static readonly ICommand ToggleInv = new ToggleInventoryModeCommand();
    private static readonly ICommand ScrollUp = new InventoryScrollCommand(-1);
    private static readonly ICommand ScrollDown = new InventoryScrollCommand(1);
    private static readonly ICommand ScrollPageUp = new InventoryScrollCommand(-5);
    private static readonly ICommand ScrollPageDn = new InventoryScrollCommand(5);
    private static readonly ICommand EquipLeft = new EquipCommand(preferLeft: true);
    private static readonly ICommand EquipRight = new EquipCommand(preferLeft: false);
    private static readonly ICommand DropItem = new DropItemCommand();

    private readonly Dictionary<ConsoleKey, ICommand> _lookup;

    public InventoryModeCommandHandler(KeyBindingMap keyMap)
    {
        _lookup = new Dictionary<ConsoleKey, ICommand>();
        Bind(keyMap, GameAction.InventoryUp, ScrollUp);
        Bind(keyMap, GameAction.InventoryDown, ScrollDown);
        Bind(keyMap, GameAction.InventoryPageUp, ScrollPageUp);
        Bind(keyMap, GameAction.InventoryPageDown, ScrollPageDn);
        Bind(keyMap, GameAction.EquipToLeft, EquipLeft);
        Bind(keyMap, GameAction.EquipToRight, EquipRight);
        Bind(keyMap, GameAction.DropItem, DropItem);
        Bind(keyMap, GameAction.CloseInventory, ToggleInv);
    }

    protected override ICommand? TryHandle(ConsoleKeyInfo key, GameState state)
    {
        if (state.CurrentMode != PlayMode.Inventory)
            return null;

        return _lookup.TryGetValue(key.Key, out var command) ? command : null;
    }

    private void Bind(KeyBindingMap keyMap, GameAction action, ICommand command)
    {
        foreach (var key in keyMap.GetKeys(action))
            _lookup[key] = command;
    }
}