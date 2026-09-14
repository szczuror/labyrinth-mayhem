using ProJob.Core;

namespace ProJob.Commands;

public sealed class FleeCombatCommand : ICommand
{
    public void Execute(Core.GameState state)
    {
        state.CurrentMode = PlayMode.Exploration;
        state.CurrentEnemy = null;
        state.Message = "Uciekasz z walki!";
    }
}
