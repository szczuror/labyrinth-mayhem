namespace ProJob.Commands;

public sealed class DropItemCommand : ICommand
{
    public void Execute(Core.GameState state)
    {
        int idx = state.SelectedInventoryIndex;
        var item = state.Player.Inventory.RemoveItem(idx, state.Player);
        if (item == null)
        {
            state.Message = "Brak przedmiotu do wyrzucenia.";
            return;
        }
        state.Board.PlaceItem(state.Player.Row, state.Player.Col, item);
        state.Message = $"Wyrzuciłeś {item.Name}.";

        int count = state.Player.Inventory.Items.Count;
        if (state.SelectedInventoryIndex >= count && count > 0)
            state.SelectedInventoryIndex = count - 1;
        else if (count == 0)
            state.SelectedInventoryIndex = 0;
    }
}
