using ProJob.Core;
using ProJob.Journal;
using BoardGrid = ProJob.Board.Board;

namespace ProJob.Commands;

public sealed class MoveCommand : ICommand
{
    private readonly int _dRow;
    private readonly int _dCol;

    public MoveCommand(int dRow, int dCol)
    {
        _dRow = dRow;
        _dCol = dCol;
    }

    public void Execute(Core.GameState state)
    {
        int newRow = state.Player.Row + _dRow;
        int newCol = state.Player.Col + _dCol;

        if (!state.Board.IsWalkable(newRow, newCol))
        {
            bool inBounds = newRow >= 0 && newRow < BoardGrid.Rows &&
                            newCol >= 0 && newCol < BoardGrid.Cols;
            if (inBounds)
                FileJournal.Instance.Log("Próba wejścia w ścianę.", state.PlayerId);
            return;
        }

        if (state.PlayerRegistry.IsOccupied(newRow, newCol, state.Player))
        {
            state.Message = "Inny gracz blokuje przejście.";
            return;
        }

        var enemy = state.Board.GetEnemy(newRow, newCol);
        if (enemy != null)
        {
            state.CurrentMode = PlayMode.Combat;
            state.CurrentEnemy = enemy;
            state.CurrentEnemyRow = newRow;
            state.CurrentEnemyCol = newCol;
            enemy.IsEngagedInCombat = true;
            state.Message = $"Walczysz z {enemy.Name}! (HP: {enemy.Health}/{enemy.MaxHealth}, Pancerz: {enemy.Armor})";
            return;
        }

        state.Player.Row = newRow;
        state.Player.Col = newCol;
    }
}
