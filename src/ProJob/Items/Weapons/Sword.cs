namespace ProJob.Items.Weapons;

public sealed class Sword : LightWeapon
{
    public override string Name => "Miecz";
    public override char Symbol => 'm';
    public override string Description => "Klasyczny jednoręczny miecz.";
    public override int Damage => 15;
    public override int HandsRequired => 1;
}
