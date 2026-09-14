using ProJob.Core;
using ProJob.Input;
using ProJob.Journal;
using System.Text;
using BoardGrid = ProJob.Board.Board;

namespace ProJob.Rendering;

public sealed class ConsoleRenderer : IGameView
{
    private const int BoardDisplayWidth = BoardGrid.Cols;
    private const int SidebarWidth = 36;
    private const int TotalWidth = BoardDisplayWidth + 2 + SidebarWidth;
    private const int BoardDisplayHeight = BoardGrid.Rows;

    private readonly StringBuilder _sb = new(TotalWidth * (BoardDisplayHeight + 20));

    private string _lastFrame = string.Empty;

    public void Initialize()
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();
    }

    public void Render(GameState state)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        _sb.Clear();

        var sidebarLines = BuildSidebarLines(state);
        int sidebarIdx = 0;

        for (int r = 0; r < BoardGrid.Rows; r++)
        {
            for (int c = 0; c < BoardGrid.Cols; c++)
            {
                AppendCellChar(_sb, state, r, c);
            }
            _sb.Append("  ");
            _sb.Append(SidebarLine(sidebarLines, sidebarIdx++));
            _sb.Append('\n');
        }

        while (sidebarIdx < sidebarLines.Count)
        {
            _sb.Append(new string(' ', BoardDisplayWidth + 2));
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
            Console.Write(new string(' ', TotalWidth));
        }

        _lastFrame = frame;
    }

    public void ShowWelcome(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine($"  {message}");
        Console.ResetColor();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  Naciśnij dowolny klawisz, aby rozpocząć...");
        Console.ResetColor();
        Console.ReadKey(intercept: true);
        Console.Clear();
    }

    public void ShowGameOver(string journalFilePath)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.CursorVisible = true;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("  KONIEC GRY - zostałeś zabity! Naciśnij dowolny klawisz...");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"  Dziennik zapisany: {journalFilePath}");
        Console.ResetColor();
    }

    public void ShowExit()
    {
        Console.CursorVisible = true;
        Console.WriteLine("Wychodzenie...");
    }

    public void ShowJournal(IReadOnlyList<string> entries)
    {
        Console.Clear();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
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
            foreach (var entry in entries)
                Console.WriteLine(entry);
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić do gry...");
        Console.ResetColor();
        Console.ReadKey(intercept: true);
        Console.Clear();
    }

    public void HideJournal() { }

    public int ShowStrategyMenu(string[] labels, KeyBindingMap keyMap)
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();

        int selected = 0;

        while (true)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Wybierz sposób generowania poziomu:\n\n");

            for (int i = 0; i < labels.Length; i++)
                Console.WriteLine(i == selected
                    ? $"  > {labels[i]}"
                    : $"    {labels[i]}");

            Console.WriteLine($"\n\n[{keyMap.GetLabel(GameAction.MenuConfirm)}] zatwierdź wybór");

            var key = Console.ReadKey(intercept: true).Key;

            if (keyMap.Matches(key, GameAction.MenuUp))
                selected = Math.Max(0, selected - 1);
            else if (keyMap.Matches(key, GameAction.MenuDown))
                selected = Math.Min(labels.Length - 1, selected + 1);
            else if (keyMap.Matches(key, GameAction.MenuConfirm))
                return selected;
        }
    }

    private static void AppendCellChar(StringBuilder sb, GameState state, int r, int c)
    {
        if (state.Player.Row == r && state.Player.Col == c)
        {
            sb.Append(state.Player.Symbol);
            return;
        }

        var enemy = state.Board.GetEnemy(r, c);
        if (enemy != null)
        {
            sb.Append(enemy.Symbol);
            return;
        }

        var items = state.Board.GetItems(r, c);
        if (items.Count > 0)
        {
            sb.Append(items[items.Count - 1].Symbol);
            return;
        }

        sb.Append(state.Board.GetCell(r, c).Symbol);
    }

    private static List<string> BuildSidebarLines(GameState state)
    {
        var lines = new List<string>(40);
        var p = state.Player;
        var inv = p.Inventory;

        string title = state.CurrentMode switch
        {
            PlayMode.Combat => "[ WALKA ]",
            PlayMode.Inventory => "[ EKWIPUNEK - tryb zarządzania ]",
            PlayMode.Exploration => "[ Labyrinth Mayhem ]",

            _ => "[ Nieznany stan ]"
        };

        lines.Add(title);

        lines.Add(new string('─', SidebarWidth));

        lines.Add($"{"Zdrowie:",-12}{p.Attributes.Health,4}  {"Siła:",-12}{p.Attributes.Strength,4}");
        lines.Add($"{"Zręczność:",-12}{p.Attributes.Dexterity,4}  {"Szczęście:",-12}{p.Attributes.Luck,4}");
        lines.Add($"{"Agresja:",-12}{p.Attributes.Aggression,4}  {"Mądrość:",-12}{p.Attributes.Wisdom,4}");
        lines.Add($"{"Monety:",-12}{p.Coins,4}  {"Złoto:",-12}{p.Gold,4}");

        if (state.CurrentMode == PlayMode.Combat && state.CurrentEnemy != null)
        {
            var e = state.CurrentEnemy;
            lines.Add(new string('─', SidebarWidth));
            lines.Add(PadOrTruncate($" Przeciwnik: {e.Name} {e.Symbol}", SidebarWidth));
            lines.Add(PadOrTruncate($"  HP: {e.Health}/{e.MaxHealth}  Atk: {e.Attack}  Pancerz: {e.Armor}", SidebarWidth));
        }

        lines.Add(new string('─', SidebarWidth));

        lines.Add(" Ręce:");
        string leftStr = inv.LeftHand != null
            ? PadOrTruncate($"  [L] {inv.LeftHand.Name}", SidebarWidth)
            : "  [L] --";
        string rightStr = inv.RightHand != null
            ? PadOrTruncate($"  [R] {inv.RightHand.Name}", SidebarWidth)
            : "  [R] --";
        lines.Add(leftStr);
        lines.Add(rightStr);

        lines.Add(new string('─', SidebarWidth));

        var cellItems = state.Board.GetItems(state.Player.Row, state.Player.Col);
        if (cellItems.Count > 0)
        {
            lines.Add(" Na ziemi:");
            foreach (var item in cellItems)
            {
                string detail = item.GetDetails();
                string line = string.IsNullOrEmpty(detail)
                    ? $"  {item.Symbol} {item.Name}"
                    : $"  {item.Symbol} {item.Name} ({detail})";
                lines.Add(PadOrTruncate(line, SidebarWidth));
            }
            lines.Add(" [E] Podnieś");
        }
        else
        {
            lines.Add(" (puste pole)");
        }

        lines.Add(new string('─', SidebarWidth));

        var items = inv.Items;
        if (items.Count == 0)
        {
            lines.Add(" Ekwipunek pusty");
        }
        else
        {
            int itemsPerPage = 5;
            int totalPages = (int)Math.Ceiling((double)items.Count / itemsPerPage);

            int currentPage = state.SelectedInventoryIndex / itemsPerPage;

            lines.Add($" Ekwipunek (Str {currentPage + 1}/{totalPages}):");

            int startIdx = currentPage * itemsPerPage;
            int endIdx = Math.Min(startIdx + itemsPerPage - 1, items.Count - 1);

            for (int i = startIdx; i <= endIdx; i++)
            {
                var item = items[i];
                bool isSelected = state.CurrentMode == PlayMode.Inventory && i == state.SelectedInventoryIndex;
                bool equipped = item.IsCurrentlyEquipped(p);
                string prefix = isSelected ? ">" : " ";
                string equipMark = equipped ? "*" : " ";
                string detail = item.GetDetails();
                string label = string.IsNullOrEmpty(detail)
                    ? $"{item.Name}"
                    : $"{item.Name} [{detail}]";

                lines.Add(PadOrTruncate($" {prefix}{equipMark}[{i + 1}] {label}", SidebarWidth));
            }

            int displayedItems = endIdx - startIdx + 1;
            for (int i = displayedItems; i < itemsPerPage; i++)
            {
                lines.Add(new string(' ', SidebarWidth));
            }
        }

        lines.Add(new string('─', SidebarWidth));

        lines.Add(" Sterowanie:");
        var instructions = state.CurrentMode switch
        {
            PlayMode.Combat => state.CombatModeInstructions,
            PlayMode.Inventory => state.InventoryModeInstructions,
            PlayMode.Exploration => state.GameModeInstructions,
            _ => []
        };

        foreach (var instruction in instructions)
            lines.Add(instruction);

        if (!string.IsNullOrEmpty(state.Message))
        {
            lines.Add(new string('─', SidebarWidth));

            string[] words = state.Message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string currentLine = " ";

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > SidebarWidth)
                {
                    lines.Add(PadOrTruncate(currentLine, SidebarWidth));
                    currentLine = "  " + word;
                }
                else
                {
                    currentLine += (currentLine.Length == 1 ? "" : " ") + word;
                }
            }

            if (currentLine.TrimEnd().Length > 0)
            {
                lines.Add(PadOrTruncate(currentLine, SidebarWidth));
            }
        }

        lines.Add(new string('─', SidebarWidth));
        lines.Add(" Dziennik (ostatnie wpisy):");
        var recent = FileJournal.Instance.GetRecent(4);
        if (recent.Count == 0)
        {
            lines.Add("  (brak wpisów)");
        }
        else
        {
            foreach (var entry in recent)
                lines.Add(PadOrTruncate("  " + entry, SidebarWidth));
        }

        return lines;
    }

    private static string SidebarLine(List<string> lines, int idx)
    {
        if (idx >= lines.Count)
            return new string(' ', SidebarWidth);
        return PadOrTruncate(lines[idx], SidebarWidth);
    }

    private static string PadOrTruncate(string s, int width)
    {
        if (s.Length >= width) return s[..width];
        return s + new string(' ', width - s.Length);
    }

    private static int CountLines(string s)
    {
        int count = 0;
        foreach (char c in s)
            if (c == '\n') count++;
        return count;
    }
}
