using ProJob.Input;
using ProJob.Rendering;

namespace ProJob.Builder.Strategies;

public static class StrategyMenu
{
    private static readonly (string Label, IDungeonBuildStrategy Strategy)[] _options =
    [
        ("Klasyczny loch", new DungeonTerrainStrategy()),
        ("Puste pokoje",   new RoomStrategy()),
        ("Łyse pole",      new SholeStrategy()),
    ];

    public static IDungeonBuildStrategy Show(KeyBindingMap keyMap, IGameView view)
    {
        int idx = view.ShowStrategyMenu(
            _options.Select(o => o.Label).ToArray(), keyMap);
        return _options[idx].Strategy;
    }
}
