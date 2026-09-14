namespace ProJob.Player;

public sealed class PlayerRegistry
{
    private readonly List<Player> _players = new();
    private readonly object _lock = new();

    public void Register(Player player)
    {
        lock (_lock) _players.Add(player);
    }

    public void Unregister(Player player)
    {
        lock (_lock) _players.Remove(player);
    }

    public bool IsOccupied(int row, int col, Player exclude)
    {
        lock (_lock)
        {
            foreach (var p in _players)
            {
                if (ReferenceEquals(p, exclude)) continue;
                if (p.Row == row && p.Col == col) return true;
            }
            return false;
        }
    }
}
