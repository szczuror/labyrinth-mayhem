using ProJob.Combat;
using ProJob.Core;
using ProJob.Input;

namespace ProJob.Commands.Handlers;

public sealed class CombatModeCommandHandler : CommandHandler
{
    private static readonly ICommand Flee = new FleeCombatCommand();

    private static readonly ICommand AttackNormal =
        new AttackCommand(attrs => new NormalCombatVisitor(attrs));
    private static readonly ICommand AttackStealth =
        new AttackCommand(attrs => new StealthCombatVisitor(attrs));
    private static readonly ICommand AttackMagic =
        new AttackCommand(attrs => new MagicCombatVisitor(attrs));

    private readonly Dictionary<ConsoleKey, ICommand> _lookup;

    public CombatModeCommandHandler(KeyBindingMap keyMap)
    {
        _lookup = new Dictionary<ConsoleKey, ICommand>();
        Bind(keyMap, GameAction.AttackNormal, AttackNormal);
        Bind(keyMap, GameAction.AttackStealth, AttackStealth);
        Bind(keyMap, GameAction.AttackMagic, AttackMagic);
        Bind(keyMap, GameAction.Flee, Flee);
    }

    protected override ICommand? TryHandle(ConsoleKeyInfo key, GameState state)
    {
        if (state.CurrentMode != PlayMode.Combat) return null;
        return _lookup.TryGetValue(key.Key, out var command) ? command : null;
    }

    private void Bind(KeyBindingMap keyMap, GameAction action, ICommand command)
    {
        foreach (var key in keyMap.GetKeys(action))
            _lookup[key] = command;
    }
}
