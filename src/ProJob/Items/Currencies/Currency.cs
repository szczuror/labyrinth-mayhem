namespace ProJob.Items.Currencies;

public abstract class Currency : Item
{
    public override char Symbol => '$';
    public abstract void CollectCurrency(Player.Player player);

    public override bool OnPickedUp(Player.Player player)
    {
        CollectCurrency(player);
        return true;
    }
}
