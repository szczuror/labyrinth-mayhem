using ProJob.Combat;
using ProJob.Items;

namespace ProJob.Board;

public sealed class Board
{
    public const int Rows = 20;
    public const int Cols = 40;

    private readonly Cell[,] _cells = new Cell[Rows, Cols];
    private readonly List<IItem>[,] _items = new List<IItem>[Rows, Cols];
    private readonly Dictionary<(int, int), Enemy> _enemies = new();

    public Board()
    {
        var empty = new EmptyCell();
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
            {
                _cells[r, c] = empty;
                _items[r, c] = new List<IItem>();
            }
    }

    public Cell GetCell(int row, int col) => _cells[row, col];

    public void SetCell(int row, int col, Cell cell) => _cells[row, col] = cell;

    public bool IsWalkable(int row, int col)
        => row >= 0 && row < Rows && col >= 0 && col < Cols
        && _cells[row, col].IsWalkable;

    public IReadOnlyList<IItem> GetItems(int row, int col) => _items[row, col];

    public void AddItem(int row, int col, IItem item) => _items[row, col].Add(item);

    public IItem? TakeTopItem(int row, int col)
    {
        var list = _items[row, col];
        if (list.Count == 0) return null;
        var item = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return item;
    }

    public void PlaceItem(int row, int col, IItem item) => _items[row, col].Insert(0, item);

    public void AddEnemy(int row, int col, Enemy enemy)
    {
        _enemies[(row, col)] = enemy;
        enemy.Row = row;
        enemy.Col = col;
    }

    public Enemy? GetEnemy(int row, int col) => _enemies.TryGetValue((row, col), out var e) ? e : null;
    public void RemoveEnemy(int row, int col) => _enemies.Remove((row, col));
    public bool HasEnemy(int row, int col) => _enemies.ContainsKey((row, col));

    public void MoveEnemy(int oldRow, int oldCol, int newRow, int newCol)
    {
        if (!_enemies.TryGetValue((oldRow, oldCol), out var enemy)) return;
        _enemies.Remove((oldRow, oldCol));
        _enemies[(newRow, newCol)] = enemy;
        enemy.Row = newRow;
        enemy.Col = newCol;
    }

    public IEnumerable<((int Row, int Col) Pos, Enemy Enemy)> GetAllEnemies()
        => _enemies.Select(kv => (kv.Key, kv.Value));
}
