using ProJob.Core;
using ProJob.Input;
using ProJob.Rendering;

namespace ProJob.Commands.Handlers;

public sealed class GameModeCommandHandler : CommandHandler
{
    private static readonly ICommand MoveUp = new MoveCommand(-1, 0);
    private static readonly ICommand MoveDown = new MoveCommand(1, 0);
    private static readonly ICommand MoveLeft = new MoveCommand(0, -1);
    private static readonly ICommand MoveRight = new MoveCommand(0, 1);
    private static readonly ICommand PickUp = new PickUpCommand();
    private static readonly ICommand ToggleInv = new ToggleInventoryModeCommand();
    private static readonly ICommand UnequipLeft = new UnequipLeftCommand();
    private static readonly ICommand UnequipRight = new UnequipRightCommand();
    private static readonly ICommand Quit = new QuitCommand();

    private readonly ICommand _viewJournal;
    private readonly Dictionary<ConsoleKey, ICommand> _lookup;

    public GameModeCommandHandler(KeyBindingMap keyMap, IGameView view)
    {
        _viewJournal = new ViewJournalCommand(view);

        _lookup = new Dictionary<ConsoleKey, ICommand>();
        Bind(keyMap, GameAction.MoveUp, MoveUp);
        Bind(keyMap, GameAction.MoveDown, MoveDown);
        Bind(keyMap, GameAction.MoveLeft, MoveLeft);
        Bind(keyMap, GameAction.MoveRight, MoveRight);
        Bind(keyMap, GameAction.PickUp, PickUp);
        Bind(keyMap, GameAction.ToggleInventory, ToggleInv);
        Bind(keyMap, GameAction.UnequipLeft, UnequipLeft);
        Bind(keyMap, GameAction.UnequipRight, UnequipRight);
        Bind(keyMap, GameAction.Quit, Quit);
        Bind(keyMap, GameAction.ViewJournal, _viewJournal);
    }

    protected override ICommand? TryHandle(ConsoleKeyInfo key, GameState state)
    {
        if (state.CurrentMode != PlayMode.Exploration)
            return null;

        return _lookup.TryGetValue(key.Key, out var command) ? command : null;
    }

    private void Bind(KeyBindingMap keyMap, GameAction action, ICommand command)
    {
        foreach (var key in keyMap.GetKeys(action))
            _lookup[key] = command;
    }
}
