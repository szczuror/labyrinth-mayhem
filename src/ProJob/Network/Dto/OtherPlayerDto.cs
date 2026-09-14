namespace ProJob.Network.Dto;

public sealed class OtherPlayerDto
{
    public int PlayerId { get; set; }
    public char Symbol { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
}
