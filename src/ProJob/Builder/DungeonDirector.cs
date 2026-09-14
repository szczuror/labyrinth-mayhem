using ProJob.Builder.Strategies;
using ProJob.Events;
using ProJob.Input;
using ProJob.Themes;

namespace ProJob.Builder;

using BoardGrid = Board.Board;

public static class DungeonDirector
{
    public static (BoardGrid Board, (int Row, int Col) Spawn, InstructionsBuilder Instructions, DungeonEventBus EventBus)
        Build(IDungeonBuildStrategy strategy, KeyBindingMap keyMap)
    {
        var map = new DungeonBuilder();
        var inst = new InstructionsBuilder(keyMap);

        strategy.Apply(map);
        strategy.Apply(inst);

        return (map.Build(), map.GetRandomSpawnPoint(), inst, map.GetEventBus());
    }

    public static (BoardGrid Board, (int Row, int Col) Spawn, InstructionsBuilder Instructions, string WelcomeMessage, DungeonEventBus EventBus)
        Build(IDungeonThemeFactory theme, KeyBindingMap keyMap)
    {
        var map = new DungeonBuilder();
        var inst = new InstructionsBuilder(keyMap);

        theme.Template.Apply(map);
        theme.Template.Apply(inst);

        foreach (var group in theme.SpeciesGroups)
        {
            map.WithSpeciesGroup(group.MinCount, group.Factory);
            inst.WithSpeciesGroup(group.MinCount, group.Factory);
        }

        map.WithThemeItems(theme.ItemCount, theme.ItemFactories)
           .WithThemeEnemies(theme.EnemyCount, theme.EnemyFactories)
           .WithArtifact(theme.ArtifactFactory);

        inst.WithThemeItems(1, theme.ItemFactories)
            .WithThemeEnemies(1, theme.EnemyFactories)
            .WithArtifact(theme.ArtifactFactory);

        return (map.Build(), map.GetRandomSpawnPoint(), inst, theme.WelcomeMessage, map.GetEventBus());
    }
}