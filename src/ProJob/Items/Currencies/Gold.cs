namespace ProJob.Items.Currencies;

public sealed class Gold : Currency
{
    public override string Name => "Złoto";
    public override char Symbol => 'g';
    public override string Description => "Sztabka złota.";
    public override string GetDetails(IItem? context = null) => "Waluta: złoto";

    public override void CollectCurrency(Player.Player player)
        => player.AddGold(1);
}
