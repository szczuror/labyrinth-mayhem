namespace ProJob.Commands;

public interface ICommand
{
    void Execute(Core.GameState state);
}
