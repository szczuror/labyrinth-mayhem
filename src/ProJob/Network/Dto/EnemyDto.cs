namespace ProJob.Network.Dto;

public sealed class EnemyDto
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Attack { get; set; }
    public int Armor { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsAlive { get; set; }
}
