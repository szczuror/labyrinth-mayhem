namespace ProJob.Network.Messages;

public interface INetworkMessageHandler
{
    void Handle(HandshakeMessage msg);
    void Handle(StateUpdateMessage msg);
    void Handle(NoiseEventMessage msg);
    void Handle(PlayerLeftMessage msg);
    void Handle(ActionMessage msg);
    void Handle(JournalLogMessage msg);
}
