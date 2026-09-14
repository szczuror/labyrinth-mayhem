namespace ProJob.Items.Weapons;

public sealed class GreatSword : HeavyWeapon
{
    public override string Name => "Wielki miecz";
    public override char Symbol => 'M';
    public override string Description => "Dwuręczny wielki miecz.";
    public override int Damage => 35;
    public override int HandsRequired => 2;
}
