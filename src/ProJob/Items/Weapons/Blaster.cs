namespace ProJob.Items.Weapons;

public sealed class Blaster : HeavyWeapon
{
    public override string Name => "Blaster";
    public override char Symbol => 'B';
    public override string Description => "Futurystyczna broń miotająca plazmą.";
    public override int Damage => 30;
    public override int HandsRequired => 2;
}
