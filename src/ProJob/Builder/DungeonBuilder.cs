using ProJob.Board;
using ProJob.Builder.Strategies;
using ProJob.Combat;
using ProJob.Events;
using ProJob.Items;
using ProJob.Items.Collectibles;
using ProJob.Items.Currencies;
using ProJob.Items.Modifiers;
using ProJob.Items.Weapons;

namespace ProJob.Builder;

using BoardGrid = Board.Board;

public sealed class DungeonBuilder : IDungeonBuilder
{
    private BoardGrid _board = new();
    private readonly Random _random = new();
    private bool _initialized;
    private HashSet<(int, int)> _walkable = new();

    private readonly DungeonEventBus _eventBus = new();
    private readonly Species _defaultGoblinSpecies = new("Gobliny", new CowardlyDeathBehavior());
    private readonly Species _defaultSkeletonSpecies = new("Szkielety", new AggressiveDeathBehavior());

    private const int GuaranteedSpeciesCount = 4;

    public DungeonEventBus GetEventBus() => _eventBus;

    public IDungeonBuilder WithEmptyDungeon()
    {
        _board = new BoardGrid();
        for (int r = 0; r < BoardGrid.Rows; r++)
            for (int c = 0; c < BoardGrid.Cols; c++)
            {
                _board.SetCell(r, c, new EmptyCell());
                _walkable.Add((r, c));
            }
        _initialized = true;
        return this;
    }

    public IDungeonBuilder WithFilledDungeon()
    {
        _board = new BoardGrid();
        for (int r = 0; r < BoardGrid.Rows; r++)
            for (int c = 0; c < BoardGrid.Cols; c++)
            {
                _board.SetCell(r, c, new WallCell());
                _walkable.Remove((r, c));
            }
        _initialized = true;
        return this;
    }

    public IDungeonBuilder WithCorridors(int count = 8)
    {
        EnsureInitialized();

        int mazeRows = (BoardGrid.Rows - 1) / 2;
        int mazeCols = (BoardGrid.Cols - 1) / 2;
        bool[,] visited = new bool[mazeRows, mazeCols];

        int startMr = _random.Next(mazeRows);
        int startMc = _random.Next(mazeCols);
        int remaining = count;

        CarveMaze(startMr, startMc, visited, mazeRows, mazeCols, ref remaining);
        return this;
    }

    public IDungeonBuilder WithRooms(int count = 4)
    {
        EnsureInitialized();
        for (int i = 0; i < count; i++)
        {
            int roomW = _random.Next(3, 7);
            int roomH = _random.Next(3, 5);
            int row = _random.Next(1, BoardGrid.Rows - roomH - 1);
            int col = _random.Next(1, BoardGrid.Cols - roomW - 1);
            for (int r = row; r < row + roomH; r++)
                for (int c = col; c < col + roomW; c++)
                {
                    _board.SetCell(r, c, new EmptyCell());
                    _walkable.Add((r, c));
                }
        }
        return this;
    }

    public IDungeonBuilder WithCentralHall(int width = 8, int height = 5)
    {
        EnsureInitialized();
        int startRow = (BoardGrid.Rows - height) / 2;
        int startCol = (BoardGrid.Cols - width) / 2;
        for (int r = startRow; r < startRow + height; r++)
            for (int c = startCol; c < startCol + width; c++)
            {
                _board.SetCell(r, c, new EmptyCell());
                _walkable.Add((r, c));
            }
        return this;
    }

    public IDungeonBuilder WithItems(int count = 5)
    {
        EnsureInitialized();
        Func<IItem>[] factories =
        [
            () => new OldBook(),
            () => new Gem(),
            () => new Amulet()
        ];
        PlaceItemsOnFloor(count, factories, _walkable);
        return this;
    }

    public IDungeonBuilder WithWeapons(int count = 3)
    {
        EnsureInitialized();
        Func<IItem>[] factories =
        [
            () => ApplyRandomModifier(new Sword()),
            () => ApplyRandomModifier(new Dagger()),
            () => ApplyRandomModifier(new GreatSword()),
            () => ApplyRandomModifier(new Battleaxe()),
            () => ApplyRandomModifier(new Staff()),
        ];
        PlaceItemsOnFloor(count, factories, _walkable);
        return this;
    }

    public IDungeonBuilder WithCurrencies(int count = 3)
    {
        EnsureInitialized();
        Func<IItem>[] factories =
        [
            () => new Coin(),
            () => new Gold()
        ];
        PlaceItemsOnFloor(count, factories, _walkable);
        return this;
    }

    public IDungeonBuilder WithEnemies(int count = 3)
    {
        EnsureInitialized();

        for (int i = 0; i < 2; i++)
        {
            (int row, int col) = _walkable.ElementAt(_random.Next(_walkable.Count));
            _board.AddEnemy(row, col, new Goblin(_defaultGoblinSpecies, _eventBus));
        }
        for (int i = 0; i < 2; i++)
        {
            (int row, int col) = _walkable.ElementAt(_random.Next(_walkable.Count));
            _board.AddEnemy(row, col, new Skeleton(_defaultSkeletonSpecies, _eventBus));
        }

        Func<Enemy>[] others =
        [
            () => new Ork(_eventBus),
            () => new Troll(_eventBus)
        ];
        for (int placed = GuaranteedSpeciesCount; placed < count; placed++)
        {
            (int row, int col) = _walkable.ElementAt(_random.Next(_walkable.Count));
            _board.AddEnemy(row, col, others[_random.Next(others.Length)]());
        }
        return this;
    }

    public IDungeonBuilder WithThemeItems(int count, IReadOnlyList<Func<IItem>> factories)
    {
        EnsureInitialized();
        if (factories.Count == 0) return this;
        PlaceItemsOnFloor(count, [.. factories], _walkable);
        return this;
    }

    public IDungeonBuilder WithThemeEnemies(int count, IReadOnlyList<Func<DungeonEventBus, Enemy>> factories)
    {
        EnsureInitialized();
        if (factories.Count == 0) return this;
        for (int placed = 0; placed < count; placed++)
        {
            (int row, int col) = _walkable.ElementAt(_random.Next(_walkable.Count));
            _board.AddEnemy(row, col, factories[_random.Next(factories.Count)](_eventBus));
        }
        return this;
    }

    public IDungeonBuilder WithSpeciesGroup(int minCount, Func<DungeonEventBus, Enemy> factory)
    {
        EnsureInitialized();
        for (int i = 0; i < minCount; i++)
        {
            (int row, int col) = _walkable.ElementAt(_random.Next(_walkable.Count));
            _board.AddEnemy(row, col, factory(_eventBus));
        }
        return this;
    }

    public IDungeonBuilder WithArtifact(Func<IItem> artifactFactory)
    {
        EnsureInitialized();
        if (_walkable.Count == 0) return this;
        (int row, int col) = _walkable.ElementAt(_random.Next(_walkable.Count));
        _board.AddItem(row, col, artifactFactory());
        return this;
    }


    public IDungeonBuilder Apply(IDungeonBuildStrategy strategy)
    {
        strategy.Apply(this);
        return this;
    }


    public BoardGrid Build()
    {
        EnsureInitialized();
        return _board;
    }


    private void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException(
                "DungeonBuilder must be initialised with WithEmptyDungeon() or WithFilledDungeon().");
    }

    private void PlaceItemsOnFloor(int count, Func<IItem>[] factories, HashSet<(int, int)> tiles)
    {
        for (int placed = 0; placed < count; placed++)
        {
            (int row, int col) = tiles.ElementAt(_random.Next(tiles.Count));
            _board.AddItem(row, col, factories[_random.Next(factories.Length)]());
        }
    }

    private void CarveMaze(int mazeRow, int mazeCol, bool[,] visited, int mazeRows, int mazeCols, ref int remaining)
    {
        visited[mazeRow, mazeCol] = true;

        int boardRow = (mazeRow * 2) + 1;
        int boardCol = (mazeCol * 2) + 1;
        _board.SetCell(boardRow, boardCol, new EmptyCell());
        _walkable.Add((boardRow, boardCol));

        var directions = new (int dRow, int dCol)[]
        {
            (-1,  0),
            ( 0,  1),
            ( 1,  0),
            ( 0, -1)
        };

        var shuffledDirs = directions.OrderBy(_ => _random.Next());

        foreach (var (dRow, dCol) in shuffledDirs)
        {
            if (remaining <= 0) return;
            int nextMazeRow = mazeRow + dRow;
            int nextMazeCol = mazeCol + dCol;

            if (nextMazeRow >= 0 && nextMazeRow < mazeRows &&
                nextMazeCol >= 0 && nextMazeCol < mazeCols &&
                !visited[nextMazeRow, nextMazeCol])
            {
                int wallRow = boardRow + dRow;
                int wallCol = boardCol + dCol;

                _board.SetCell(wallRow, wallCol, new EmptyCell());
                _walkable.Add((wallRow, wallCol));
                remaining--;

                CarveMaze(nextMazeRow, nextMazeCol, visited, mazeRows, mazeCols, ref remaining);
            }
        }
    }

    private IItem ApplyRandomModifier(IItem item)
    {
        Func<IItem, IItem>[] modifiers =
        [
            i => new UnluckyModifier(i),
            i => new StrongModifier(i),
            i => new ProtectiveModifier(i),
            i => i,
            i => i,
        ];
        return modifiers[_random.Next(modifiers.Length)](item);
    }

    public (int Row, int Col) GetRandomSpawnPoint()
    {
        EnsureInitialized();

        if (_walkable.Count == 0)
            return (0, 0);

        return _walkable.ElementAt(_random.Next(_walkable.Count));
    }

}
