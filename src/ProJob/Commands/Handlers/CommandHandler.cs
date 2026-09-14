namespace ProJob.Commands.Handlers;

public abstract class CommandHandler
{
    private CommandHandler? _next;

    public CommandHandler SetNext(CommandHandler next)
    {
        _next = next;
        return next;
    }

    public ICommand? Handle(ConsoleKeyInfo key, Core.GameState state)
    {
        var command = TryHandle(key, state);
        if (command != null)
            return command;
        return _next?.Handle(key, state);
    }

    protected abstract ICommand? TryHandle(ConsoleKeyInfo key, Core.GameState state);
}
