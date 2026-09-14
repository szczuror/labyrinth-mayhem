using ProJob.Combat;
using ProJob.Commands;
using ProJob.Core;
using ProJob.Journal;
using ProJob.Network.Messages;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using GameBoard = ProJob.Board.Board;

namespace ProJob.Network;

public sealed class GameServer
{
    private const int MaxPlayers = 9;

    private const int MinActionIntervalMs = 150;

    private readonly int _port;
    private readonly ConcurrentDictionary<int, ConnectedClient> _clients = new();
    private readonly Dictionary<int, GameState> _playerStates = new();
    private readonly Dictionary<int, DateTime> _lastActionTime = new();
    private readonly Queue<int> _freePlayerIds = new(Enumerable.Range(1, MaxPlayers));
    private readonly object _stateLock = new();
    private readonly Channel<(int PlayerId, ActionMessage Action)> _actionChannel =
        Channel.CreateUnbounded<(int, ActionMessage)>();
    private readonly Random _random = new();

    private GameState _sharedState = null!;
    private NetworkNoiseObserver _noiseObserver = null!;
    private NetworkJournalObserver _journalObserver = null!;

    private readonly Dictionary<string, Func<ICommand>> _commandFactories;

    private static readonly Dictionary<PlayMode, HashSet<string>> AllowedActionsByMode = new()
    {
        [PlayMode.Exploration] = new HashSet<string>
        {
            "MoveUp", "MoveDown", "MoveLeft", "MoveRight",
            "PickUp", "ToggleInventory",
            "UnequipLeft", "UnequipRight",
        },
        [PlayMode.Inventory] = new HashSet<string>
        {
            "InventoryUp", "InventoryDown",
            "InventoryPageUp", "InventoryPageDown",
            "EquipLeft", "EquipRight",
            "DropItem", "ToggleInventory",
        },
        [PlayMode.Combat] = new HashSet<string>
        {
            "AttackNormal", "AttackStealth", "AttackMagic", "Flee",
        },
    };

    public GameServer(int port)
    {
        _port = port;
        _commandFactories = new Dictionary<string, Func<ICommand>>
        {
            ["MoveUp"] = () => new MoveCommand(-1, 0),
            ["MoveDown"] = () => new MoveCommand(1, 0),
            ["MoveLeft"] = () => new MoveCommand(0, -1),
            ["MoveRight"] = () => new MoveCommand(0, 1),
            ["PickUp"] = () => new PickUpCommand(),
            ["AttackNormal"] = () => new AttackCommand(a => new NormalCombatVisitor(a)),
            ["AttackStealth"] = () => new AttackCommand(a => new StealthCombatVisitor(a)),
            ["AttackMagic"] = () => new AttackCommand(a => new MagicCombatVisitor(a)),
            ["Flee"] = () => new FleeCombatCommand(),
            ["ToggleInventory"] = () => new ToggleInventoryModeCommand(),
            ["EquipLeft"] = () => new EquipCommand(preferLeft: true),
            ["EquipRight"] = () => new EquipCommand(preferLeft: false),
            ["UnequipLeft"] = () => new UnequipLeftCommand(),
            ["UnequipRight"] = () => new UnequipRightCommand(),
            ["DropItem"] = () => new DropItemCommand(),
            ["InventoryUp"] = () => new InventoryScrollCommand(-1),
            ["InventoryDown"] = () => new InventoryScrollCommand(1),
            ["InventoryPageUp"] = () => new InventoryScrollCommand(-5),
            ["InventoryPageDown"] = () => new InventoryScrollCommand(5),
        };
    }

    public async Task RunAsync(GameState initialState, CancellationToken ct = default)
    {
        _sharedState = initialState;
        _noiseObserver = new NetworkNoiseObserver(_clients, SendMessageAsync);
        _sharedState.EventBus.Subscribe(_noiseObserver);
        _journalObserver = new NetworkJournalObserver(_clients, SendMessageAsync);
        FileJournal.Instance.Subscribe(_journalObserver);

        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        Console.WriteLine($"[Serwer] Nasłuchuje na porcie {_port}. Czeka na graczy (max {MaxPlayers})...");

        try
        {
            var acceptTask = AcceptClientsAsync(listener, ct);
            var processTask = ProcessActionsAsync(ct);
            await Task.WhenAll(acceptTask, processTask);
        }
        finally
        {
            _sharedState.EventBus.Unsubscribe(_noiseObserver);
            FileJournal.Instance.Unsubscribe(_journalObserver);
            listener.Stop();
        }
    }

    private async Task AcceptClientsAsync(TcpListener listener, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            bool slotAvailable;
            lock (_stateLock) { slotAvailable = _freePlayerIds.Count > 0; }
            if (!slotAvailable)
            {
                await Task.Delay(500, ct);
                continue;
            }

            TcpClient tcp;
            try
            {
                tcp = await listener.AcceptTcpClientAsync(ct);
            }
            catch (OperationCanceledException) { break; }

            int playerId = -1;
            lock (_stateLock)
            {
                if (_freePlayerIds.Count > 0)
                    playerId = _freePlayerIds.Dequeue();
            }

            if (playerId < 0)
            {
                tcp.Dispose();
                continue;
            }

            Console.WriteLine($"[Serwer] Gracz {playerId} połączony: {tcp.Client.RemoteEndPoint}");

            _ = Task.Run(() => HandleClientAsync(tcp, playerId, ct), ct);
        }
    }

    private async Task HandleClientAsync(TcpClient tcp, int playerId, CancellationToken ct)
    {
        ProJob.Player.Player player;
        ConnectedClient client;

        lock (_stateLock)
        {
            var spawnPos = FindSpawnPoint();
            player = new ProJob.Player.Player { Row = spawnPos.Row, Col = spawnPos.Col };

            var playerState = new GameState(_sharedState.Board, player)
            {
                GameModeInstructions = _sharedState.GameModeInstructions,
                InventoryModeInstructions = _sharedState.InventoryModeInstructions,
                CombatModeInstructions = _sharedState.CombatModeInstructions,
                PlayerName = $"Gracz {playerId}",
                WelcomeMessage = _sharedState.WelcomeMessage,
                EventBus = _sharedState.EventBus,
                PlayerRegistry = _sharedState.PlayerRegistry,
                PlayerId = playerId,
            };

            _sharedState.PlayerRegistry.Register(player);
            _playerStates[playerId] = playerState;
            client = new ConnectedClient(playerId, tcp, player);
            _clients[playerId] = client;
        }

        try
        {
            IReadOnlyDictionary<int, GameState> snapshot;
            lock (_stateLock) { snapshot = new Dictionary<int, GameState>(_playerStates); }
            var handshakeDto = GameStateSerializer.ToMultiPlayerDto(snapshot[playerId], snapshot, playerId);
            await SendMessageAsync(client, new HandshakeMessage(playerId, handshakeDto));

            await BroadcastStateAsync(excludePlayerId: playerId);

            var actionRouter = new ServerActionRouter(playerId, _actionChannel);
            while (!ct.IsCancellationRequested)
            {
                string? line = await client.Reader.ReadLineAsync(ct);
                if (line == null) break;

                try
                {
                    var msg = GameStateSerializer.DeserializeMessage(line);
                    msg?.Execute(actionRouter);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine($"[Serwer] Gracz {playerId} wysłał uszkodzony JSON: {ex.Message}. Rozłączam.");
                    break;
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Serwer] Gracz {playerId} rozłączył się: {ex.Message}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[Serwer] Gracz {playerId} rozłączył się: {ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"[Serwer] Gracz {playerId} rozłączył się: {ex.Message}");
        }
        finally
        {
            lock (_stateLock)
            {
                if (_playerStates.TryGetValue(playerId, out var ps))
                {
                    _sharedState.PlayerRegistry.Unregister(ps.Player);
                    ps.CurrentEnemy = null;
                }
                _clients.TryRemove(playerId, out _);
                _playerStates.Remove(playerId);
                _lastActionTime.Remove(playerId);
                _freePlayerIds.Enqueue(playerId);
                ReconcileCombatState();
            }
            tcp.Dispose();
            Console.WriteLine($"[Serwer] Gracz {playerId} usunięty (slot zwolniony).");
            _ = BroadcastMessageAsync(new PlayerLeftMessage(playerId));
            _ = BroadcastStateAsync();
        }
    }

    private async Task ProcessActionsAsync(CancellationToken ct)
    {
        await foreach (var (playerId, action) in _actionChannel.Reader.ReadAllAsync(ct))
        {
            try
            {
                await ProcessActionAsync(playerId, action);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Serwer] Błąd przetwarzania akcji gracza {playerId}: {ex.Message}");
            }
        }
    }

    private async Task ProcessActionAsync(int playerId, ActionMessage action)
    {
        bool playerDied = false;

        lock (_stateLock)
        {
            if (!_playerStates.TryGetValue(playerId, out var playerState)) return;

            if (playerState.Player.Attributes.Health <= 0) return;

            var now = DateTime.UtcNow;
            if (_lastActionTime.TryGetValue(playerId, out var last)
                && (now - last).TotalMilliseconds < MinActionIntervalMs)
            {
                return;
            }
            _lastActionTime[playerId] = now;

            if (!AllowedActionsByMode.TryGetValue(playerState.CurrentMode, out var allowed)
                || !allowed.Contains(action.Action))
            {
                return;
            }

            if (_commandFactories.TryGetValue(action.Action, out var factory))
            {
                factory().Execute(playerState);
            }

            ReconcileCombatState();

            if (playerState.CurrentMode == PlayMode.Exploration)
                MoveEnemiesRandomly();

            playerDied = playerState.Player.Attributes.Health <= 0;
        }

        await BroadcastStateAsync();

        if (playerDied && _clients.TryGetValue(playerId, out var deadClient))
        {
            Console.WriteLine($"[Serwer] Gracz {playerId} zginął — zamykam połączenie.");
            try { deadClient.Tcp.Close(); }
            catch (Exception ex) { Console.WriteLine($"[Serwer] Close({playerId}) błąd: {ex.Message}"); }
        }
    }

    private void ReconcileCombatState()
    {
        foreach (var ps in _playerStates.Values)
        {
            if (ps.CurrentEnemy != null && !ps.CurrentEnemy.IsAlive)
            {
                var dead = ps.CurrentEnemy;
                ps.CurrentEnemy = null;
                ps.CurrentMode = PlayMode.Exploration;
                ps.Message = $"{dead.Name} został pokonany.";
            }
        }

        var engaged = new HashSet<Enemy>(ReferenceEqualityComparer.Instance);
        foreach (var ps in _playerStates.Values)
        {
            if (ps.CurrentEnemy != null) engaged.Add(ps.CurrentEnemy);
        }

        foreach (var (_, enemy) in _sharedState.Board.GetAllEnemies())
        {
            enemy.IsEngagedInCombat = engaged.Contains(enemy);
        }
    }

    private async Task BroadcastStateAsync(int excludePlayerId = -1)
    {
        var pending = new List<(ConnectedClient Client, StateUpdateMessage Msg)>(_clients.Count);

        lock (_stateLock)
        {
            foreach (var (id, client) in _clients)
            {
                if (id == excludePlayerId) continue;
                if (!_playerStates.TryGetValue(id, out var playerState)) continue;

                var dto = GameStateSerializer.ToMultiPlayerDto(playerState, _playerStates, id);
                pending.Add((client, new StateUpdateMessage(dto)));
            }
        }

        var tasks = pending.Select(p => SendMessageAsync(p.Client, p.Msg));
        await Task.WhenAll(tasks);
    }

    private async Task BroadcastMessageAsync(INetworkMessage msg)
    {
        var tasks = _clients.Values.Select(c => SendMessageAsync(c, msg));
        await Task.WhenAll(tasks);
    }

    public async Task SendMessageAsync(ConnectedClient client, INetworkMessage msg)
    {
        string json = GameStateSerializer.SerializeMessage(msg);
        await client.SendLock.WaitAsync();
        try
        {
            await client.Writer.WriteLineAsync(json);
            await client.Writer.FlushAsync();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Serwer] Błąd zapisu do klienta {client.PlayerId}: {ex.Message}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[Serwer] Błąd zapisu do klienta {client.PlayerId}: {ex.Message}");
        }
        finally
        {
            client.SendLock.Release();
        }
    }

    private (int Row, int Col) FindSpawnPoint()
    {
        var occupiedByPlayers = new HashSet<(int, int)>(
            _playerStates.Values.Select(s => (s.Player.Row, s.Player.Col)));

        for (int r = 0; r < GameBoard.Rows; r++)
            for (int c = 0; c < GameBoard.Cols; c++)
                if (_sharedState.Board.IsWalkable(r, c)
                 && !occupiedByPlayers.Contains((r, c))
                 && _sharedState.Board.GetEnemy(r, c) == null)
                    return (r, c);

        for (int r = 0; r < GameBoard.Rows; r++)
            for (int c = 0; c < GameBoard.Cols; c++)
                if (_sharedState.Board.IsWalkable(r, c))
                    return (r, c);

        return (1, 1);
    }

    private void MoveEnemiesRandomly()
    {
        var board = _sharedState.Board;
        var allEnemies = board.GetAllEnemies().ToList();
        var occupied = new HashSet<(int, int)>(allEnemies.Select(e => e.Pos));
        var playerSet = new HashSet<(int, int)>(
            _playerStates.Values.Select(s => (s.Player.Row, s.Player.Col)));

        foreach (var (pos, enemy) in allEnemies)
        {
            if (enemy.IsEngagedInCombat) continue;

            var candidates = new (int Row, int Col)[]
            {
                (pos.Row - 1, pos.Col),
                (pos.Row + 1, pos.Col),
                (pos.Row, pos.Col - 1),
                (pos.Row, pos.Col + 1),
            }
            .Where(p => board.IsWalkable(p.Row, p.Col)
                     && !playerSet.Contains((p.Row, p.Col))
                     && !occupied.Contains((p.Row, p.Col)))
            .ToList();

            if (candidates.Count == 0) continue;

            var newPos = candidates[_random.Next(candidates.Count)];
            occupied.Remove((pos.Row, pos.Col));
            occupied.Add((newPos.Row, newPos.Col));
            board.MoveEnemy(pos.Row, pos.Col, newPos.Row, newPos.Col);
        }
    }
}
