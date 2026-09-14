namespace ProJob.Items.Collectibles;

public abstract class Collectible : Item
{
    public override string GetDetails(IItem? context = null) => Description;
}
