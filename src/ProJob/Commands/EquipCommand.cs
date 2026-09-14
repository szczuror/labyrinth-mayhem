namespace ProJob.Commands;

using ProJob.Journal;

public sealed class EquipCommand : ICommand
{
    private readonly bool _preferLeft;

    public EquipCommand(bool preferLeft) => _preferLeft = preferLeft;

    public void Execute(Core.GameState state)
    {
        int idx = state.SelectedInventoryIndex;
        var items = state.Player.Inventory.Items;
        if (idx < 0 || idx >= items.Count)
        {
            state.Message = "Wybierz przedmiot z ekwipunku.";
            return;
        }
        var item = items[idx];

        bool success = item.TryEquip(state.Player, _preferLeft);

        if (success)
        {
            string hand = item.HandsRequired == 2 ? "obu rąk" : _preferLeft ? "lewej ręki" : "prawej ręki";
            state.Message = $"Wyposażyłeś {item.Name} do {hand}.";
            FileJournal.Instance.Log($"Wzięto do ręki: {item.Name} ({hand}).", state.PlayerId);
        }
        else
        {
            state.Message = $"{item.Name} nie może być wyposażony.";
        }
    }
}