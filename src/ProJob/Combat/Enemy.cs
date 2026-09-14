using ProJob.Events;
using ProJob.Journal;
using ProJob.Observer;

namespace ProJob.Combat;

public abstract class Enemy : IGameObserver<Enemy>, IGameObserver<NoiseEvent>
{
    public string Name { get; }
    public char Symbol { get; }
    public int Health { get; protected set; }
    public int MaxHealth { get; }
    public int Attack { get; }
    public int Armor { get; }

    public int Row { get; set; }
    public int Col { get; set; }

    private int _attackBonus;
    private int _armorBonus;

    private const int MinEffectiveAttack = 1;

    public int EffectiveAttack => Math.Max(MinEffectiveAttack, Attack + _attackBonus);
    public int EffectiveArmor => Math.Max(0, Armor + _armorBonus);

    private readonly Species? _species;
    private readonly DungeonEventBus? _eventBus;

    public bool IsAlive => Health > 0;

    public bool IsEngagedInCombat { get; set; }

    protected Enemy(string name, char symbol, int health, int attack, int armor,
        Species? species = null, DungeonEventBus? eventBus = null)
    {
        Name = name;
        Symbol = symbol;
        Health = MaxHealth = health;
        Attack = attack;
        Armor = armor;
        _species = species;
        _eventBus = eventBus;

        species?.Register(this);
        eventBus?.Subscribe(this);
    }

    public virtual void TakeDamage(int damage)
    {
        int actualDamage = Math.Max(0, damage - EffectiveArmor);
        Health = Math.Max(0, Health - actualDamage);
    }

    public virtual int PerformAttack() => EffectiveAttack;

    public void ModifyAttack(int delta) => _attackBonus += delta;

    public void ModifyArmor(int delta) => _armorBonus += delta;

    public void Die()
    {
        _species?.NotifyDeath(this);
        _eventBus?.Unsubscribe(this);
    }

    void IGameObserver<Enemy>.OnNotify(Enemy deadAlly)
    {
        _species?.DeathBehavior.OnAllyDied(this);
        FileJournal.Instance.Log(
            $"{Name} reaguje na śmierć {deadAlly.Name} " +
            $"(skuteczny atak: {EffectiveAttack}, skuteczna zbroja: {EffectiveArmor}).");
    }

    void IGameObserver<NoiseEvent>.OnNotify(NoiseEvent noise)
    {
        if (noise.ReachablePositions.TryGetValue((Row, Col), out int distance))
            FileJournal.Instance.Log(
                $"{Name} na pozycji ({Row},{Col}) usłyszał hałas ze źródła " +
                $"({noise.SourceRow},{noise.SourceCol}), odległość: {distance}.");
    }
}