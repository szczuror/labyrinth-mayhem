using ProJob.Combat;
using ProJob.Core;
using ProJob.Items;
using ProJob.Journal;
using ProJob.Player;

namespace ProJob.Commands;

public sealed class AttackCommand : ICommand
{
    private readonly Func<PlayerAttributes, ICombatVisitor> _visitorFactory;

    public AttackCommand(Func<PlayerAttributes, ICombatVisitor> visitorFactory)
    {
        _visitorFactory = visitorFactory;
    }

    public void Execute(Core.GameState state)
    {
        var enemy = state.CurrentEnemy;
        if (enemy == null || !enemy.IsAlive) return;

        var visitor = _visitorFactory(state.Player.Attributes);

        int playerDamage = ComputeTotalAttack(state.Player.Inventory, visitor);
        int playerDefense = ComputeTotalDefense(state.Player.Inventory, visitor);

        enemy.TakeDamage(playerDamage);
        int actualDamage = Math.Max(0, playerDamage - enemy.Armor);

        FileJournal.Instance.Log($"Atak na {enemy.Name}: zadano {actualDamage} obrażeń (po pancerzu).", state.PlayerId);

        if (!enemy.IsAlive)
        {
            enemy.Die();
            state.Board.RemoveEnemy(state.CurrentEnemyRow, state.CurrentEnemyCol);
            state.CurrentMode = PlayMode.Exploration;
            state.CurrentEnemy = null;
            state.Message = $"Pokonałeś {enemy.Name}! (zadano {actualDamage} obrażeń)";
            FileJournal.Instance.Log($"Pokonano {enemy.Name}!", state.PlayerId);
            return;
        }

        int enemyDamage = Math.Max(0, enemy.PerformAttack() - playerDefense);
        state.Player.Attributes.Health -= enemyDamage;

        FileJournal.Instance.Log($"{enemy.Name} atakuje: zadano {enemyDamage} obrażeń graczowi (obrona: {playerDefense}).", state.PlayerId);

        if (state.Player.Attributes.Health <= 0)
        {
            state.Player.Attributes.Health = 0;
            state.CurrentEnemy = null;
            state.CurrentMode = PlayMode.Exploration;
            state.IsRunning = false;
            state.Message = $"Zostałeś zabity przez {enemy.Name}! KONIEC GRY";
            FileJournal.Instance.Log($"Gracz zginął od {enemy.Name}. Koniec gry.", state.PlayerId);
            return;
        }

        state.Message =
            $"Zadano {enemy.Name}: {actualDamage} (po pancerzu). " +
            $"Otrzymano: {enemyDamage} (obrona: {playerDefense}). " +
            $"HP wroga: {enemy.Health}/{enemy.MaxHealth}";
    }

    private static int ComputeTotalAttack(Inventory inventory, ICombatVisitor visitor)
    {
        int damage = 0;
        foreach (var item in GetUniqueEquipped(inventory))
            damage += item.AcceptAttack(visitor);
        return damage;
    }

    private static int ComputeTotalDefense(Inventory inventory, ICombatVisitor visitor)
    {
        var unique = GetUniqueEquipped(inventory).ToList();
        if (unique.Count == 0)
            return visitor.DefenseNone(null);

        int maxDefense = 0;
        foreach (var item in unique)
        {
            int itemDefense = item.AcceptDefense(visitor);
            if (itemDefense > maxDefense)
            {
                maxDefense = itemDefense;
            }
        }

        return maxDefense;
    }

    private static IEnumerable<IItem> GetUniqueEquipped(Inventory inventory)
    {
        var seen = new HashSet<IItem>(ReferenceEqualityComparer.Instance);
        if (inventory.LeftHand != null && seen.Add(inventory.LeftHand))
            yield return inventory.LeftHand;
        if (inventory.RightHand != null && seen.Add(inventory.RightHand))
            yield return inventory.RightHand;
    }
}
