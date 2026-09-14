using ProJob.Core;
using ProJob.Input;

namespace ProJob.Rendering;

public interface IGameView
{
    void Initialize();
    void Render(GameState state);
    void ShowWelcome(string message);
    void ShowGameOver(string journalFilePath);
    void ShowExit();
    void ShowJournal(IReadOnlyList<string> entries);
    void HideJournal();
    int ShowStrategyMenu(string[] labels, KeyBindingMap keyMap);
}
