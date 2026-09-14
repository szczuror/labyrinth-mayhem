using ProJob.Events;
using ProJob.Network.Messages;
using ProJob.Observer;
using System.Collections.Concurrent;

namespace ProJob.Network;

public sealed class NetworkNoiseObserver : IGameObserver<NoiseEvent>
{
    private readonly ConcurrentDictionary<int, ConnectedClient> _clients;
    private readonly Func<ConnectedClient, INetworkMessage, Task> _send;

    public NetworkNoiseObserver(
        ConcurrentDictionary<int, ConnectedClient> clients,
        Func<ConnectedClient, INetworkMessage, Task> send)
    {
        _clients = clients;
        _send = send;
    }

    public void OnNotify(NoiseEvent noise)
    {
        foreach (var (_, client) in _clients)
        {
            int row = client.Player.Row;
            int col = client.Player.Col;

            if (row == noise.SourceRow && col == noise.SourceCol) continue;

            if (noise.ReachablePositions.TryGetValue((row, col), out int distance))
            {
                var msg = new NoiseEventMessage(noise.SourceRow, noise.SourceCol, noise.Range, distance);
                _ = _send(client, msg);
            }
        }
    }
}
