namespace ProJob.Items.Currencies;

public sealed class Coin : Currency
{
    public override string Name => "Moneta";
    public override char Symbol => 'c';
    public override string Description => "Zwykła moneta.";
    public override string GetDetails(IItem? context = null) => "Waluta: moneta";

    public override void CollectCurrency(Player.Player player)
        => player.AddCoin(1);
}
