namespace ProJob.Board;

public sealed class EmptyCell : Cell
{
    public override char Symbol => ' ';
    public override bool IsWalkable => true;
}
