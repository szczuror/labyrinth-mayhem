using ProJob.Builder;
using ProJob.Config;
using ProJob.Core;
using ProJob.Input;
using ProJob.Journal;
using ProJob.Network;
using ProJob.Network.Args;
using ProJob.Rendering;
using ProJob.Themes;

NetworkArgs netArgs = args.Length > 0
    ? NetworkArgs.Parse(args)
    : NetworkArgs.PromptUser();

if (netArgs.Mode == NetworkMode.Client)
{
    var networkView = new NetworkClientView();
    await new GameClient(netArgs.Host, netArgs.Port, networkView).RunAsync();
    return;
}

var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
var config = GameConfigLoader.Load(configPath);

var startTime = DateTime.Now;
FileJournal.Initialize(config.PlayerName, config.JournalSavePath, startTime);
FileJournal.Instance.Log($"Rozpoczęto rozgrywkę jako '{config.PlayerName}'. Motyw: {config.DungeonTheme}.");

var keyMap = KeyBindingMap.CreateDefault();

var theme = ThemeRegistry.Get(config.DungeonTheme);
var (board, spawn, inst, welcomeMessage, eventBus) = DungeonDirector.Build(theme, keyMap);

var player = new ProJob.Player.Player { Row = spawn.Row, Col = spawn.Col };
var state = new GameState(board, player)
{
    GameModeInstructions = inst.BuildGameModeInstructions(),
    InventoryModeInstructions = inst.BuildInventoryModeInstructions(),
    CombatModeInstructions = inst.BuildCombatModeInstructions(),
    PlayerName = config.PlayerName,
    WelcomeMessage = welcomeMessage,
    EventBus = eventBus,
};

if (netArgs.Mode == NetworkMode.Server)
{
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
    try { await new GameServer(netArgs.Port).RunAsync(state, cts.Token); }
    catch (OperationCanceledException) { }
    Console.WriteLine("[Serwer] Zamknięto.");
    return;
}

IGameView view = new ConsoleRenderer();
IInputSource input = new ConsoleInputSource();
new GameLoop(state, keyMap, view, input).Run();
