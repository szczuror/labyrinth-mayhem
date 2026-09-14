namespace ProJob.Items.Modifiers;

public sealed class UnluckyModifier : ItemDecorator
{
    public UnluckyModifier(IItem inner) : base(inner) { }

    protected override string ModifierName => "Pechowy";

    public override void ApplyEquipEffect(Player.Player player)
    {
        base.ApplyEquipEffect(player);
        player.Attributes.Luck -= 5;
    }

    public override void RemoveEquipEffect(Player.Player player)
    {
        base.RemoveEquipEffect(player);
        player.Attributes.Luck += 5;
    }
}
