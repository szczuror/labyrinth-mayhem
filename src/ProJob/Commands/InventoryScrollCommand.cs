namespace ProJob.Commands;

public sealed class InventoryScrollCommand : ICommand
{
    private readonly int _amount;

    public InventoryScrollCommand(int amount) => _amount = amount;

    public void Execute(Core.GameState state)
    {
        int maxIndex = state.Player.Inventory.Items.Count - 1;
        if (maxIndex < 0) return;

        int newIndex = state.SelectedInventoryIndex + _amount;

        if (newIndex < 0) newIndex = 0;
        else if (newIndex > maxIndex) newIndex = maxIndex;

        state.SelectedInventoryIndex = newIndex;
    }
}