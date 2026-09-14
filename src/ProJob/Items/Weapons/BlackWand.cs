namespace ProJob.Items.Weapons;

public sealed class BlackWand : MagicWeapon
{
    public override string Name => "Czarna Różdżka";
    public override char Symbol => '|';
    public override string Description => "Tajemnicza różdżka emanująca mroczną mocą.";
    public override int Damage => 25;
    public override int HandsRequired => 1;

    public override void ApplyEquipEffect(Player.Player player)
        => player.Attributes.Wisdom += 10;

    public override void RemoveEquipEffect(Player.Player player)
        => player.Attributes.Wisdom -= 10;
}
