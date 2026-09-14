namespace ProJob.Items.Weapons;

public sealed class Dagger : LightWeapon
{
    public override string Name => "Sztylet";
    public override char Symbol => '!';
    public override string Description => "Jednoręczny sztylet.";
    public override int Damage => 8;
    public override int HandsRequired => 1;
}
