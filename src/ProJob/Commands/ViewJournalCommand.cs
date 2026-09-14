using ProJob.Core;
using ProJob.Journal;
using ProJob.Rendering;

namespace ProJob.Commands;

public sealed class ViewJournalCommand : ICommand
{
    private readonly IGameView _view;

    public ViewJournalCommand(IGameView view)
    {
        _view = view;
    }

    public void Execute(GameState state)
    {
        _view.ShowJournal(FileJournal.Instance.GetAll());
    }
}
