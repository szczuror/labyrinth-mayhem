namespace ProJob.Player;

public sealed class Player
{
    public int Row { get; set; }
    public int Col { get; set; }

    public PlayerAttributes Attributes { get; } = new();
    public Inventory Inventory { get; } = new();

    public int Coins { get; private set; }
    public int Gold { get; private set; }

    public char Symbol => '¶';

    public void AddCoin(int amount)
    {
        Coins += amount;
    }
    public void AddGold(int amount)
    {
        Gold += amount;
    }
}
