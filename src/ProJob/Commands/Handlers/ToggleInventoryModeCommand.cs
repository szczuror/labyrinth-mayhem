using ProJob.Core;

namespace ProJob.Commands.Handlers;

public sealed class ToggleInventoryModeCommand : ICommand
{
    public void Execute(Core.GameState state)
    {
        state.CurrentMode = state.CurrentMode == PlayMode.Inventory
        ? PlayMode.Exploration
        : PlayMode.Inventory;

        state.Message = state.CurrentMode == PlayMode.Inventory
        ? "Otworzyłeś ekwipunek."
        : "Zamknąłeś ekwipunek.";
    }
}
