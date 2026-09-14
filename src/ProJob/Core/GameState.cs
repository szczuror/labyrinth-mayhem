using ProJob.Combat;
using ProJob.Events;

namespace ProJob.Core;

public sealed class GameState
{
    public Board.Board Board { get; }
    public Player.Player Player { get; }
    public PlayMode CurrentMode { get; set; } = PlayMode.Exploration;

    public int SelectedInventoryIndex { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsRunning { get; set; } = true;

    public Enemy? CurrentEnemy { get; set; }
    public int CurrentEnemyRow { get; set; }
    public int CurrentEnemyCol { get; set; }

    public IReadOnlyList<string> GameModeInstructions { get; init; } = [];
    public IReadOnlyList<string> InventoryModeInstructions { get; init; } = [];
    public IReadOnlyList<string> CombatModeInstructions { get; init; } = [];

    public string PlayerName { get; init; } = "Bohater";
    public string WelcomeMessage { get; init; } = string.Empty;

    public int? PlayerId { get; init; }

    public DungeonEventBus EventBus { get; init; } = new();

    public Player.PlayerRegistry PlayerRegistry { get; init; } = new();

    public GameState(Board.Board board, Player.Player player)
    {
        Board = board;
        Player = player;
    }
}
