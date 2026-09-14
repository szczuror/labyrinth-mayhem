using ProJob.Observer;

namespace ProJob.Events;

public sealed class DungeonEventBus : Observable<NoiseEvent>
{
    public void PublishNoise(Board.Board board, int sourceRow, int sourceCol, int range)
    {
        if (range <= 0) return;
        var reachable = ComputeReachable(board, sourceRow, sourceCol, range);
        Notify(new NoiseEvent(sourceRow, sourceCol, range, reachable));
    }

    private static Dictionary<(int, int), int> ComputeReachable(
        Board.Board board, int startRow, int startCol, int range)
    {
        var distances = new Dictionary<(int, int), int>();
        var queue = new Queue<(int Row, int Col, int Dist)>();

        distances[(startRow, startCol)] = 0;
        queue.Enqueue((startRow, startCol, 0));

        while (queue.Count > 0)
        {
            var (row, col, dist) = queue.Dequeue();
            if (dist >= range) continue;

            foreach (var (dr, dc) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                int nr = row + dr, nc = col + dc;
                if (board.IsWalkable(nr, nc) && !distances.ContainsKey((nr, nc)))
                {
                    distances[(nr, nc)] = dist + 1;
                    queue.Enqueue((nr, nc, dist + 1));
                }
            }
        }

        return distances;
    }
}
