namespace ProJob.Items.Collectibles;

public sealed class WisdomBook : Item
{
    public override string Name => "Księga mądrości";
    public override char Symbol => 'W';
    public override string Description => "Stara księga pełna wiedzy. Dodaje +5 do mądrości.";

    public override bool OnPickedUp(Player.Player player)
    {
        player.Attributes.Wisdom += 5;
        return true;
    }
}
