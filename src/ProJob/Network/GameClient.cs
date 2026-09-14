using ProJob.Journal;
using ProJob.Network.Messages;
using ProJob.Rendering;
using System.Net.Sockets;
using System.Text;

namespace ProJob.Network;

public sealed class GameClient : INetworkMessageHandler
{
    private readonly string _host;
    private readonly int _port;
    private readonly NetworkClientView _view;
    private readonly CancellationTokenSource _cts = new();

    private StreamReader _reader = null!;
    private StreamWriter _writer = null!;
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private int _myPlayerId;
    private volatile string _currentMode = "Exploration";
    private volatile bool _viewingJournal;

    private static readonly IReadOnlyDictionary<ConsoleKey, string> ExplorationKeys =
        new Dictionary<ConsoleKey, string>
        {
            [ConsoleKey.W] = "MoveUp",
            [ConsoleKey.UpArrow] = "MoveUp",
            [ConsoleKey.S] = "MoveDown",
            [ConsoleKey.DownArrow] = "MoveDown",
            [ConsoleKey.A] = "MoveLeft",
            [ConsoleKey.LeftArrow] = "MoveLeft",
            [ConsoleKey.D] = "MoveRight",
            [ConsoleKey.RightArrow] = "MoveRight",
            [ConsoleKey.E] = "PickUp",
            [ConsoleKey.I] = "ToggleInventory",
            [ConsoleKey.F] = "UnequipLeft",
            [ConsoleKey.G] = "UnequipRight",
        };

    private static readonly IReadOnlyDictionary<ConsoleKey, string> InventoryKeys =
        new Dictionary<ConsoleKey, string>
        {
            [ConsoleKey.W] = "InventoryUp",
            [ConsoleKey.UpArrow] = "InventoryUp",
            [ConsoleKey.S] = "InventoryDown",
            [ConsoleKey.DownArrow] = "InventoryDown",
            [ConsoleKey.LeftArrow] = "InventoryPageUp",
            [ConsoleKey.RightArrow] = "InventoryPageDown",
            [ConsoleKey.L] = "EquipLeft",
            [ConsoleKey.R] = "EquipRight",
            [ConsoleKey.D] = "DropItem",
            [ConsoleKey.I] = "ToggleInventory",
            [ConsoleKey.Q] = "ToggleInventory",
            [ConsoleKey.Escape] = "ToggleInventory",
        };

    private static readonly IReadOnlyDictionary<ConsoleKey, string> CombatKeys =
        new Dictionary<ConsoleKey, string>
        {
            [ConsoleKey.D1] = "AttackNormal",
            [ConsoleKey.D2] = "AttackStealth",
            [ConsoleKey.D3] = "AttackMagic",
            [ConsoleKey.Escape] = "Flee",
        };

    private readonly IReadOnlyDictionary<ConsoleKey, Func<Task>> _clientActions;

    public GameClient(string host, int port, NetworkClientView view)
    {
        _host = host;
        _port = port;
        _view = view;
        _clientActions = new Dictionary<ConsoleKey, Func<Task>>
        {
            [ConsoleKey.J] = ToggleJournalAsync,
        };
    }

    private Task ToggleJournalAsync()
    {
        if (_currentMode != "Exploration") return Task.CompletedTask;
        if (_viewingJournal)
        {
            _viewingJournal = false;
            _view.HideJournal();
        }
        else
        {
            _viewingJournal = true;
            _view.ShowJournal(FileJournal.Instance.GetAll());
        }
        return Task.CompletedTask;
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"[Klient] Łączenie z {_host}:{_port}...");

        try
        {
            using var tcp = new TcpClient();
            await tcp.ConnectAsync(_host, _port);

            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            _reader = new StreamReader(tcp.GetStream(), encoding, leaveOpen: true);
            _writer = new StreamWriter(tcp.GetStream(), encoding, leaveOpen: true) { AutoFlush = false };

            Console.WriteLine("[Klient] Połączono. Oczekiwanie na handshake...");

            string? firstLine = await _reader.ReadLineAsync(_cts.Token);
            if (firstLine == null) { Console.WriteLine("[Klient] Serwer zamknął połączenie."); return; }

            INetworkMessage? firstMsg;
            try
            {
                firstMsg = GameStateSerializer.DeserializeMessage(firstLine);
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"[Klient] Otrzymano uszkodzony handshake od serwera: {ex.Message}");
                return;
            }

            _view.Initialize();
            firstMsg?.Execute(this);

            var receiveTask = Task.Run(() => ReceiveLoopAsync(_cts.Token), _cts.Token);
            var inputTask = Task.Run(() => InputLoopAsync(_cts.Token), _cts.Token);

            await Task.WhenAny(receiveTask, inputTask);
            _cts.Cancel();
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[Klient] Błąd połączenia: {ex.Message}");
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Console.WriteLine($"[Klient] Błąd: {ex.Message}");
        }
        finally
        {
            _view.ShowExit();
        }
    }


    public void Handle(HandshakeMessage msg)
    {
        _myPlayerId = msg.YourPlayerId;
        _currentMode = msg.State.CurrentMode;
        FileJournal.Initialize($"Klient_Gracz{_myPlayerId}", AppContext.BaseDirectory, DateTime.Now);
        _view.SetPlayerId(_myPlayerId);
        if (!string.IsNullOrEmpty(msg.State.WelcomeMessage))
            _view.ShowWelcome(msg.State.WelcomeMessage);
        _view.UpdateState(msg.State);
    }

    public void Handle(StateUpdateMessage msg)
    {
        _currentMode = msg.State.CurrentMode;
        _view.UpdateState(msg.State);
    }

    public void Handle(NoiseEventMessage msg) => _view.ShowNoiseEvent(msg);

    public void Handle(PlayerLeftMessage msg) => _view.ShowPlayerLeft(msg.PlayerId);

    public void Handle(ActionMessage msg) { }

    public void Handle(JournalLogMessage msg) => FileJournal.Instance.Append(msg.Text);

    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                string? line = await _reader.ReadLineAsync(ct);
                if (line == null) break;

                try
                {
                    var msg = GameStateSerializer.DeserializeMessage(line);
                    msg?.Execute(this);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine($"[Klient] Otrzymano uszkodzone dane od serwera: {ex.Message}. Rozłączam.");
                    break;
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Klient] Połączenie zakończone: {ex.Message}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[Klient] Połączenie zakończone: {ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"[Klient] Połączenie zakończone: {ex.Message}");
        }
    }

    private async Task InputLoopAsync(CancellationToken ct)
    {
        await Task.Yield();
        while (!ct.IsCancellationRequested)
        {
            ConsoleKeyInfo keyInfo;
            try
            {
                keyInfo = Console.ReadKey(intercept: true);
            }
            catch (InvalidOperationException) { break; }

            if (ct.IsCancellationRequested) break;

            if (_viewingJournal)
            {
                _viewingJournal = false;
                _view.HideJournal();
                continue;
            }

            if (_clientActions.TryGetValue(keyInfo.Key, out var clientAction))
            {
                await clientAction();
                continue;
            }

            var keyMap = _currentMode switch
            {
                "Inventory" => InventoryKeys,
                "Combat" => CombatKeys,
                _ => ExplorationKeys,
            };

            if (keyMap.TryGetValue(keyInfo.Key, out string? action))
                await SendMessageAsync(new ActionMessage(action));
        }
    }

    private async Task SendMessageAsync(INetworkMessage msg)
    {
        await _sendLock.WaitAsync();
        try
        {
            string json = GameStateSerializer.SerializeMessage(msg);
            await _writer.WriteLineAsync(json);
            await _writer.FlushAsync();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[Klient] Błąd wysyłania: {ex.Message}");
            _cts.Cancel();
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"[Klient] Błąd wysyłania: {ex.Message}");
            _cts.Cancel();
        }
        finally
        {
            _sendLock.Release();
        }
    }
}
