using ProJob.Input;
using ProJob.Journal;
using ProJob.Rendering;

namespace ProJob.Core;

public sealed class GameLoop
{
    private readonly GameState _state;
    private readonly IGameView _view;
    private readonly IInputSource _input;
    private readonly GameModeDetector _modeDetector;
    private readonly Random _random = new();

    public GameLoop(GameState state, KeyBindingMap keyMap, IGameView view, IInputSource input)
    {
        _state = state;
        _view = view;
        _input = input;
        _modeDetector = new GameModeDetector(keyMap, view);
    }

    public void Run()
    {
        _view.Initialize();
        _view.ShowWelcome(_state.WelcomeMessage);

        while (_state.IsRunning)
        {
            _view.Render(_state);
            _state.Message = string.Empty;

            var key = _input.ReadKeyInfo();
            var chain = _modeDetector.GetChainFor(_state);
            var command = chain.Handle(key, _state);
            command?.Execute(_state);

            if (_state.IsRunning && _state.CurrentMode == PlayMode.Exploration)
                MoveEnemiesRandomly();
        }

        _view.Render(_state);

        if (_state.Player.Attributes.Health <= 0)
        {
            FileJournal.Instance.Log($"Gracz {_state.PlayerName} poległ. Koniec gry.");
            _view.ShowGameOver(FileJournal.Instance.FilePath);
        }
        else
        {
            _view.ShowExit();
        }
    }

    private void MoveEnemiesRandomly()
    {
        var allEnemies = _state.Board.GetAllEnemies().ToList();
        var occupied = new HashSet<(int, int)>(allEnemies.Select(e => e.Pos));
        var playerPos = (_state.Player.Row, _state.Player.Col);

        foreach (var (pos, enemy) in allEnemies)
        {
            var candidates = new (int Row, int Col)[]
            {
                (pos.Row - 1, pos.Col),
                (pos.Row + 1, pos.Col),
                (pos.Row, pos.Col - 1),
                (pos.Row, pos.Col + 1),
            }
            .Where(p => _state.Board.IsWalkable(p.Row, p.Col)
                     && (p.Row, p.Col) != playerPos
                     && !occupied.Contains((p.Row, p.Col)))
            .ToList();

            if (candidates.Count == 0) continue;

            var newPos = candidates[_random.Next(candidates.Count)];
            occupied.Remove((pos.Row, pos.Col));
            occupied.Add((newPos.Row, newPos.Col));
            _state.Board.MoveEnemy(pos.Row, pos.Col, newPos.Row, newPos.Col);
        }
    }
}
