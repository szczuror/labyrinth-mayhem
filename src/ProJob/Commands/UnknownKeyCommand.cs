using ProJob.Core;
using ProJob.Journal;

namespace ProJob.Commands;

public sealed class UnknownKeyCommand : ICommand
{
    private readonly ConsoleKeyInfo _key;

    public UnknownKeyCommand(ConsoleKeyInfo key) => _key = key;

    public void Execute(GameState state)
    {
        state.Message = $"Nieznany klawisz: '{_key.KeyChar}'.";
        FileJournal.Instance.Log($"Wciśnięto nieznany klawisz: '{_key.KeyChar}'.", state.PlayerId);
    }
}