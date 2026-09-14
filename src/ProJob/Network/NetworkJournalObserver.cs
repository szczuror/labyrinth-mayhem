using ProJob.Network.Messages;
using ProJob.Observer;
using System.Collections.Concurrent;

namespace ProJob.Network;

public sealed class NetworkJournalObserver : IGameObserver<string>
{
    private readonly ConcurrentDictionary<int, ConnectedClient> _clients;
    private readonly Func<ConnectedClient, INetworkMessage, Task> _send;

    public NetworkJournalObserver(
        ConcurrentDictionary<int, ConnectedClient> clients,
        Func<ConnectedClient, INetworkMessage, Task> send)
    {
        _clients = clients;
        _send = send;
    }

    public void OnNotify(string entry)
    {
        foreach (var (_, client) in _clients)
            _ = _send(client, new JournalLogMessage(entry));
    }
}
