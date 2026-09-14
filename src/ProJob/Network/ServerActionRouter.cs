using ProJob.Network.Messages;
using System.Threading.Channels;

namespace ProJob.Network;

internal sealed class ServerActionRouter : INetworkMessageHandler
{
    private readonly int _playerId;
    private readonly ChannelWriter<(int PlayerId, ActionMessage Action)> _writer;

    public ServerActionRouter(
        int playerId,
        Channel<(int PlayerId, ActionMessage Action)> channel)
    {
        _playerId = playerId;
        _writer = channel.Writer;
    }

    public void Handle(HandshakeMessage msg) { }
    public void Handle(StateUpdateMessage msg) { }
    public void Handle(NoiseEventMessage msg) { }
    public void Handle(PlayerLeftMessage msg) { }

    public void Handle(ActionMessage msg) => _writer.TryWrite((_playerId, msg));

    // Journal entries are produced server-side and broadcast to clients;
    // clients never send them upstream.
    public void Handle(JournalLogMessage msg) { }
}
