namespace ProJob.Items.Modifiers;

public sealed class StrongModifier : ItemDecorator
{
    public StrongModifier(IItem inner) : base(inner) { }

    protected override string ModifierName => "Silny";
    public override int Damage => Inner.Damage + 5;
}
