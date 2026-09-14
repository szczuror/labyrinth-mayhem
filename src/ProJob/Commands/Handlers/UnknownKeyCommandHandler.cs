using ProJob.Core;

namespace ProJob.Commands.Handlers;

public sealed class UnknownKeyCommandHandler : CommandHandler
{
    protected override ICommand? TryHandle(ConsoleKeyInfo key, GameState state)
    {
        return new UnknownKeyCommand(key);
    }
}
