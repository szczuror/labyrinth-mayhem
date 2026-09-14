namespace ProJob.Commands;

public sealed class UnequipRightCommand : ICommand
{
    public void Execute(Core.GameState state)
    {
        var item = state.Player.Inventory.RightHand;
        if (item == null)
        {
            state.Message = "Prawa ręka jest pusta.";
            return;
        }
        item.Unequip(state.Player);
        state.Message = $"Schowałeś {item.Name} z prawej ręki.";
    }
}
