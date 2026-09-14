namespace ProJob.Commands;

public sealed class QuitCommand : ICommand
{
    public void Execute(Core.GameState state)
    {
        state.IsRunning = false;
    }
}
