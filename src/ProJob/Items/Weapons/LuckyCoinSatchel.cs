namespace ProJob.Items.Weapons;

public sealed class LuckyCoinSatchel : HeavyWeapon
{
    public override string Name => "Szczęśliwa Sakwa Monet";
    public override char Symbol => '$';
    public override string Description => "Ciężka sakwa pełna złotych monet. Nie wiesz co z nią robić, ale jakoś trafia do obu rąk.";
    public override int Damage => 20;
    public override int HandsRequired => 2;

    public override void ApplyEquipEffect(Player.Player player)
        => player.Attributes.Luck += 15;

    public override void RemoveEquipEffect(Player.Player player)
        => player.Attributes.Luck -= 15;
}
