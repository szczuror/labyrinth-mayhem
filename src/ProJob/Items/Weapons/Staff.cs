namespace ProJob.Items.Weapons;

public sealed class Staff : MagicWeapon
{
    public override string Name => "Laska";
    public override char Symbol => '/';
    public override string Description => "Magiczna laska bojowa.";
    public override int Damage => 12;
    public override int HandsRequired => 2;
}
