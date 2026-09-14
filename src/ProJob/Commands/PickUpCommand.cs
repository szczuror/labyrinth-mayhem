namespace ProJob.Commands;

using ProJob.Journal;

public sealed class PickUpCommand : ICommand
{
    public void Execute(Core.GameState state)
    {
        var item = state.Board.TakeTopItem(state.Player.Row, state.Player.Col);
        if (item == null)
        {
            state.Message = "Nie ma tu nic do podniesienia.";
            return;
        }

        bool success = item.OnPickedUp(state.Player);

        if (success)
        {
            state.Message = $"Zebrano {item.Name}.";
            FileJournal.Instance.Log($"Podniesiono: {item.Name}.", state.PlayerId);

            if (item.NoiseLevel > 0)
            {
                state.EventBus.PublishNoise(
                    state.Board, state.Player.Row, state.Player.Col, item.NoiseLevel);
                FileJournal.Instance.Log(
                    $"Podniesienie {item.Name} wygenerowało hałas o zasięgu {item.NoiseLevel}.",
                    state.PlayerId);
            }
        }
        else
        {
            state.Board.PlaceItem(state.Player.Row, state.Player.Col, item);
            state.Message = $"Nie można podnieść {item.Name}.";
        }
    }
}
