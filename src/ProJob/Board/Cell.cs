namespace ProJob.Board;

public abstract class Cell
{
    public abstract char Symbol { get; }

    public abstract bool IsWalkable { get; }
}
