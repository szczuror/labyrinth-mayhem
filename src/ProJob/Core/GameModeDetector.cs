using ProJob.Commands.Handlers;
using ProJob.Input;
using ProJob.Rendering;

namespace ProJob.Core;

public sealed class GameModeDetector
{
    private readonly CommandHandler _gameChain;
    private readonly CommandHandler _inventoryChain;
    private readonly CommandHandler _combatChain;

    public GameModeDetector(KeyBindingMap keyMap, IGameView view)
    {
        var gameHandler = new GameModeCommandHandler(keyMap, view);
        gameHandler.SetNext(new UnknownKeyCommandHandler());
        _gameChain = gameHandler;

        var invHandler = new InventoryModeCommandHandler(keyMap);
        invHandler.SetNext(new UnknownKeyCommandHandler());
        _inventoryChain = invHandler;

        var combatHandler = new CombatModeCommandHandler(keyMap);
        combatHandler.SetNext(new UnknownKeyCommandHandler());
        _combatChain = combatHandler;
    }

    public CommandHandler GetChainFor(GameState state)
    {
        return state.CurrentMode switch
        {
            PlayMode.Combat => _combatChain,
            PlayMode.Inventory => _inventoryChain,
            PlayMode.Exploration => _gameChain,
            _ => _gameChain
        };
    }
}
