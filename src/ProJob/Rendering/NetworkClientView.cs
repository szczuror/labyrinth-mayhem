using ProJob.Core;
using ProJob.Input;
using ProJob.Network.Dto;
using ProJob.Network.Messages;
using System.Text;
using BoardGrid = ProJob.Board.Board;

namespace ProJob.Rendering;

public sealed class NetworkClientView : IGameView
{
    private const int BoardW = BoardGrid.Cols;
    private const int BoardH = BoardGrid.Rows;
    private const int SidebarW = 36;
    private const int TotalW = BoardW + 2 + SidebarW;

    private MultiPlayerStateDto? _state;
    private int _myPlayerId;
    private string _pendingMessage = string.Empty;
    private readonly object _renderLock = new();
    private string _lastFrame = string.Empty;
    private readonly StringBuilder _sb = new(TotalW * (BoardH + 20));
    private IReadOnlyList<string>? _journalEntries;

    public void SetPlayerId(int playerId) => _myPlayerId = playerId;

    public void UpdateState(MultiPlayerStateDto dto)
    {
        lock (_renderLock)
        {
            _state = dto;
            _pendingMessage = string.Empty;
            if (_journalEntries == null)
                RenderBoard();
        }
    }

    public void ShowNoiseEvent(NoiseEventMessage msg)
    {
        lock (_renderLock)
        {
            _pendingMessage = $"[HAŁAS] źródło: ({msg.SourceRow},{msg.SourceCol}), odl.: {msg.Distance}";
            RenderBoard();
        }
    }

    public void ShowPlayerLeft(int playerId)
    {
        lock (_renderLock)
        {
            _pendingMessage = $"Gracz {playerId} opuścił grę.";
            if (_state != null)
                RenderBoard();
            else
                Console.WriteLine(_pendingMessage);
        }
    }

    public void Initialize()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Clear();
    }

    public void Render(GameState state) { }

    public void ShowWelcome(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {message}");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  Naciśnij dowolny klawisz, aby rozpocząć...");
        Console.ReadKey(intercept: true);
        Console.Clear();
    }

    public void ShowGameOver(string journalFilePath)
    {
        Console.CursorVisible = true;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("  KONIEC GRY! Naciśnij dowolny klawisz...");
        Console.ResetColor();
    }

    public void ShowExit()
    {
        Console.CursorVisible = true;
        Console.WriteLine("\nRozłączono z serwerem.");
    }

    public void ShowJournal(IReadOnlyList<string> entries)
    {
        lock (_renderLock)
        {
            _journalEntries = entries;
            RenderJournal();
        }
    }

    public void HideJournal()
    {
        lock (_renderLock)
        {
            _journalEntries = null;
            _lastFrame = string.Empty;
            Console.Clear();
            if (_state != null) RenderBoard();
        }
    }

    private void RenderJournal()
    {
        var entries = _journalEntries ?? Array.Empty<string>();
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("═══════════════════════ DZIENNIK ZDARZEŃ ═══════════════════════");
        Console.ResetColor();
        Console.WriteLine();

        if (entries.Count == 0)
        {
            Console.WriteLine("  (brak wpisów)");
        }
        else
        {
            int maxLines = Math.Max(1, Console.BufferHeight - 6);
            int start = Math.Max(0, entries.Count - maxLines);
            for (int i = start; i < entries.Count; i++)
                Console.WriteLine(entries[i]);
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("[J] – wróć do gry");
        Console.ResetColor();
        _lastFrame = string.Empty;
    }

    public int ShowStrategyMenu(string[] labels, KeyBindingMap keyMap) => 0;

    private void RenderBoard()
    {
        if (_state == null) return;

        _sb.Clear();
        var sidebarLines = BuildSidebarLines(_state);
        int sidebarIdx = 0;

        for (int r = 0; r < BoardH; r++)
        {
            for (int c = 0; c < BoardW; c++)
                AppendCell(_sb, _state, r, c);

            _sb.Append("  ");
            _sb.Append(SidebarLine(sidebarLines, sidebarIdx++));
            _sb.Append('\n');
        }

        while (sidebarIdx < sidebarLines.Count)
        {
            _sb.Append(new string(' ', BoardW + 2));
            _sb.Append(SidebarLine(sidebarLines, sidebarIdx++));
            _sb.Append('\n');
        }

        string frame = _sb.ToString();
        Console.SetCursorPosition(0, 0);
        Console.Write(frame);

        int prevLines = CountLines(_lastFrame);
        int newLines = CountLines(frame);
        for (int i = newLines; i < prevLines; i++)
        {
            if (i >= Console.BufferHeight) break;
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', TotalW));
        }

        _lastFrame = frame;
    }

    private void AppendCell(StringBuilder sb, MultiPlayerStateDto state, int r, int c)
    {
        if (state.Player.Row == r && state.Player.Col == c)
        {
            sb.Append((char)('0' + _myPlayerId));
            return;
        }

        foreach (var other in state.OtherPlayers)
        {
            if (other.Row == r && other.Col == c)
            {
                sb.Append(other.Symbol);
                return;
            }
        }

        var enemy = state.Board.Enemies.FirstOrDefault(e => e.Row == r && e.Col == c && e.IsAlive);
        if (enemy != null)
        {
            sb.Append(enemy.Symbol.Length > 0 ? enemy.Symbol[0] : '?');
            return;
        }

        var cell = state.Board.ItemCells.FirstOrDefault(ic => ic.Row == r && ic.Col == c);
        if (cell?.Items.Count > 0)
        {
            var sym = cell.Items[cell.Items.Count - 1].Symbol;
            sb.Append(sym.Length > 0 ? sym[0] : '?');
            return;
        }

        string rowStr = state.Board.Terrain[r];
        sb.Append(c < rowStr.Length ? rowStr[c] : ' ');
    }

    private List<string> BuildSidebarLines(MultiPlayerStateDto state)
    {
        var lines = new List<string>(40);
        var p = state.Player;
        var inv = p.Inventory;

        string title = state.CurrentMode switch
        {
            "Combat" => $"[ WALKA ] — Gracz {_myPlayerId}",
            "Inventory" => $"[ EKWIPUNEK ] — Gracz {_myPlayerId}",
            _ => $"[ Labyrinth Mayhem ] — Gracz {_myPlayerId}",
        };

        lines.Add(title);
        lines.Add(new string('─', SidebarW));

        lines.Add($"{"Zdrowie:",-12}{p.Attributes.Health,4}  {"Siła:",-12}{p.Attributes.Strength,4}");
        lines.Add($"{"Zręczność:",-12}{p.Attributes.Dexterity,4}  {"Szczęście:",-12}{p.Attributes.Luck,4}");
        lines.Add($"{"Agresja:",-12}{p.Attributes.Aggression,4}  {"Mądrość:",-12}{p.Attributes.Wisdom,4}");
        lines.Add($"{"Monety:",-12}{p.Coins,4}  {"Złoto:",-12}{p.Gold,4}");

        if (state.OtherPlayers.Count > 0)
        {
            lines.Add(new string('─', SidebarW));
            lines.Add(" Inni gracze:");
            foreach (var other in state.OtherPlayers)
                lines.Add(Fit($"  [{other.Symbol}] HP: {other.Health}/{other.MaxHealth}  @({other.Row},{other.Col})"));
        }

        if (state.CurrentMode == "Combat" && state.CurrentEnemy != null)
        {
            var e = state.CurrentEnemy;
            lines.Add(new string('─', SidebarW));
            lines.Add(Fit($" Przeciwnik: {e.Name} {(e.Symbol.Length > 0 ? e.Symbol[0] : '?')}"));
            lines.Add(Fit($"  HP: {e.Health}/{e.MaxHealth}  Atk: {e.Attack}  Pancerz: {e.Armor}"));
        }

        lines.Add(new string('─', SidebarW));
        string leftStr = inv.LeftHand != null ? Fit($"  [L] {ComposeName(inv.LeftHand)}") : "  [L] --";
        string rightStr = inv.RightHand != null ? Fit($"  [R] {ComposeName(inv.RightHand)}") : "  [R] --";
        lines.Add(" Ręce:"); lines.Add(leftStr); lines.Add(rightStr);

        lines.Add(new string('─', SidebarW));
        var groundCell = state.Board.ItemCells.FirstOrDefault(ic => ic.Row == p.Row && ic.Col == p.Col);
        if (groundCell != null && groundCell.Items.Count > 0)
        {
            lines.Add(" Stoisz na:");
            foreach (var item in groundCell.Items)
                lines.Add(Fit($"  {(item.Symbol.Length > 0 ? item.Symbol[0] : '?')} {ComposeName(item)}"));
            lines.Add(" [E] aby podnieść");
        }
        else
        {
            lines.Add(" (puste pole)");
        }

        if (state.CurrentMode == "Inventory")
        {
            lines.Add(new string('─', SidebarW));
            if (inv.Items.Count == 0)
            {
                lines.Add(" Ekwipunek pusty");
            }
            else
            {
                const int itemsPerPage = 5;
                int totalPages = (int)Math.Ceiling((double)inv.Items.Count / itemsPerPage);
                int currentPage = state.SelectedInventoryIndex / itemsPerPage;
                lines.Add($" Ekwipunek (Str {currentPage + 1}/{totalPages}):");

                int startIdx = currentPage * itemsPerPage;
                int endIdx = Math.Min(startIdx + itemsPerPage - 1, inv.Items.Count - 1);

                for (int i = startIdx; i <= endIdx; i++)
                {
                    var item = inv.Items[i];
                    bool isSelected = i == state.SelectedInventoryIndex;
                    bool equipped =
                        (inv.LeftHand != null && ComposeName(inv.LeftHand) == ComposeName(item)) ||
                        (inv.RightHand != null && ComposeName(inv.RightHand) == ComposeName(item));
                    string prefix = isSelected ? ">" : " ";
                    string equipMark = equipped ? "*" : " ";
                    lines.Add(Fit($" {prefix}{equipMark}[{i + 1}] {ComposeName(item)}"));
                }

                int shown = endIdx - startIdx + 1;
                for (int i = shown; i < itemsPerPage; i++)
                    lines.Add(new string(' ', SidebarW));
            }
        }

        lines.Add(new string('─', SidebarW));
        lines.Add(" Sterowanie:");
        lines.Add(" W/S/A/D – ruch   E – podnieś");
        lines.Add(" I – ekwipunek   L/R – załóż");
        lines.Add(" F – zdejmij z lewej   G – z prawej");
        lines.Add(" 1/2/3 – atak    Esc – ucieczka");
        lines.Add(" J – dziennik zdarzeń");

        string effectiveMessage = !string.IsNullOrEmpty(_pendingMessage)
            ? _pendingMessage
            : state.Message;

        if (!string.IsNullOrEmpty(effectiveMessage))
        {
            lines.Add(new string('─', SidebarW));
            foreach (var word in WrapText(effectiveMessage, SidebarW))
                lines.Add(word);
        }

        return lines;
    }

    private static string SidebarLine(List<string> lines, int idx)
        => idx < lines.Count ? Fit(lines[idx]) : new string(' ', SidebarW);

    private static string Fit(string s)
        => s.Length >= SidebarW ? s[..SidebarW] : s + new string(' ', SidebarW - s.Length);

    private static string ComposeName(ItemDto d)
        => d.Inner == null ? d.Name : $"{ComposeName(d.Inner)} ({d.Name})";

    private static IEnumerable<string> WrapText(string text, int width)
    {
        string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string current = " ";
        foreach (var word in words)
        {
            if (current.Length + word.Length + 1 > width)
            {
                yield return Fit(current);
                current = "  " + word;
            }
            else
            {
                current += (current.Length == 1 ? "" : " ") + word;
            }
        }
        if (current.TrimEnd().Length > 0)
            yield return Fit(current);
    }

    private static int CountLines(string s)
    {
        int n = 0;
        foreach (char c in s) if (c == '\n') n++;
        return n;
    }
}
