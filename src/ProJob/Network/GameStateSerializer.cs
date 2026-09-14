using ProJob.Combat;
using ProJob.Core;
using ProJob.Network.Dto;
using ProJob.Network.Messages;
using System.Text;
using System.Text.Json;
using GameBoard = ProJob.Board.Board;

namespace ProJob.Network;

public static class GameStateSerializer
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = false };

    public static string SerializeMessage(INetworkMessage msg)
        => JsonSerializer.Serialize<INetworkMessage>(msg, Options);

    public static INetworkMessage? DeserializeMessage(string line)
        => JsonSerializer.Deserialize<INetworkMessage>(line, Options);

    public static MultiPlayerStateDto ToMultiPlayerDto(
        GameState playerState,
        IReadOnlyDictionary<int, GameState> allPlayerStates,
        int targetPlayerId)
    {
        var base_ = ToDto(playerState);
        var others = new List<OtherPlayerDto>(allPlayerStates.Count - 1);

        foreach (var (id, s) in allPlayerStates)
        {
            if (id == targetPlayerId) continue;
            others.Add(new OtherPlayerDto
            {
                PlayerId = id,
                Symbol = (char)('0' + id),
                Row = s.Player.Row,
                Col = s.Player.Col,
                Health = s.Player.Attributes.Health,
                MaxHealth = 100,
            });
        }

        return new MultiPlayerStateDto
        {
            Board = base_.Board,
            Player = base_.Player,
            CurrentMode = base_.CurrentMode,
            PlayerName = base_.PlayerName,
            WelcomeMessage = base_.WelcomeMessage,
            Message = base_.Message,
            CurrentEnemy = base_.CurrentEnemy,
            CurrentEnemyRow = base_.CurrentEnemyRow,
            CurrentEnemyCol = base_.CurrentEnemyCol,
            SelectedInventoryIndex = base_.SelectedInventoryIndex,
            YourPlayerId = targetPlayerId,
            OtherPlayers = others,
        };
    }

    private static GameStateDto ToDto(GameState state) => new()
    {
        Board = BoardToDto(state.Board),
        Player = PlayerToDto(state.Player),
        CurrentMode = state.CurrentMode.ToString(),
        PlayerName = state.PlayerName,
        WelcomeMessage = state.WelcomeMessage,
        Message = state.Message,
        CurrentEnemy = state.CurrentEnemy != null ? EnemyToDto(state.CurrentEnemy) : null,
        CurrentEnemyRow = state.CurrentEnemyRow,
        CurrentEnemyCol = state.CurrentEnemyCol,
        SelectedInventoryIndex = state.SelectedInventoryIndex,
    };

    private static BoardDto BoardToDto(GameBoard board)
    {
        var terrain = new string[GameBoard.Rows];
        var itemCells = new List<CellDto>();

        for (int r = 0; r < GameBoard.Rows; r++)
        {
            var row = new StringBuilder(GameBoard.Cols);
            for (int c = 0; c < GameBoard.Cols; c++)
            {
                row.Append(board.GetCell(r, c).Symbol);
                var items = board.GetItems(r, c);
                if (items.Count > 0)
                    itemCells.Add(new CellDto { Row = r, Col = c, Items = items.Select(i => i.ToDto()).ToList() });
            }
            terrain[r] = row.ToString();
        }

        var enemies = board.GetAllEnemies()
            .Select(entry => EnemyToDto(entry.Enemy))
            .ToList();

        return new BoardDto
        {
            Rows = GameBoard.Rows,
            Cols = GameBoard.Cols,
            Terrain = terrain,
            Enemies = enemies,
            ItemCells = itemCells,
        };
    }

    private static EnemyDto EnemyToDto(Enemy e) => new()
    {
        Name = e.Name,
        Symbol = e.Symbol.ToString(),
        Health = e.Health,
        MaxHealth = e.MaxHealth,
        Attack = e.Attack,
        Armor = e.Armor,
        Row = e.Row,
        Col = e.Col,
        IsAlive = e.IsAlive,
    };

    private static PlayerDto PlayerToDto(ProJob.Player.Player p) => new()
    {
        Row = p.Row,
        Col = p.Col,
        Symbol = p.Symbol.ToString(),
        Coins = p.Coins,
        Gold = p.Gold,
        Attributes = new PlayerAttributesDto
        {
            Health = p.Attributes.Health,
            Strength = p.Attributes.Strength,
            Dexterity = p.Attributes.Dexterity,
            Luck = p.Attributes.Luck,
            Aggression = p.Attributes.Aggression,
            Wisdom = p.Attributes.Wisdom,
        },
        Inventory = new InventoryDto
        {
            Items = p.Inventory.Items.Select(i => i.ToDto()).ToList(),
            LeftHand = p.Inventory.LeftHand?.ToDto(),
            RightHand = p.Inventory.RightHand?.ToDto(),
        },
    };
}
