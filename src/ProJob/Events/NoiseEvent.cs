namespace ProJob.Events;

public sealed class NoiseEvent
{
    public int SourceRow { get; }
    public int SourceCol { get; }
    public int Range { get; }
    public IReadOnlyDictionary<(int Row, int Col), int> ReachablePositions { get; }

    public NoiseEvent(int sourceRow, int sourceCol, int range,
        IReadOnlyDictionary<(int Row, int Col), int> reachablePositions)
    {
        SourceRow = sourceRow;
        SourceCol = sourceCol;
        Range = range;
        ReachablePositions = reachablePositions;
    }
}
