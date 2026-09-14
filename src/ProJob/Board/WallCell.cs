namespace ProJob.Board;

public sealed class WallCell : Cell
{
    public override char Symbol => '█';
    public override bool IsWalkable => false;
}
