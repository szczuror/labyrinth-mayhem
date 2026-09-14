namespace ProJob.Items.Modifiers;

public sealed class ProtectiveModifier : ItemDecorator
{
    public ProtectiveModifier(IItem inner) : base(inner) { }

    protected override string ModifierName => "Ochronny";

    public override void ApplyEquipEffect(Player.Player player)
    {
        base.ApplyEquipEffect(player);
        player.Attributes.Dexterity += 5;
    }

    public override void RemoveEquipEffect(Player.Player player)
    {
        base.RemoveEquipEffect(player);
        player.Attributes.Dexterity -= 5;
    }
}
