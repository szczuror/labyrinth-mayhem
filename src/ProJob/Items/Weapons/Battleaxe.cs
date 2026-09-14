namespace ProJob.Items.Weapons;

public sealed class Battleaxe : HeavyWeapon
{
    public override string Name => "Topór";
    public override char Symbol => 'T';
    public override string Description => "Jednoręczny topór bojowy.";
    public override int Damage => 20;
    public override int HandsRequired => 1;
}
