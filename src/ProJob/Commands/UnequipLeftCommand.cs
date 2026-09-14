namespace ProJob.Commands;

public sealed class UnequipLeftCommand : ICommand
{
    public void Execute(Core.GameState state)
    {
        var item = state.Player.Inventory.LeftHand;
        if (item == null)
        {
            state.Message = "Lewa ręka jest pusta.";
            return;
        }
        item.Unequip(state.Player);
        state.Message = $"Schowałeś {item.Name} z lewej ręki.";
    }
}
