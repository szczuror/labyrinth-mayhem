using System.Text.Json.Serialization;

namespace ProJob.Network.Messages;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$msgType")]
[JsonDerivedType(typeof(HandshakeMessage), "handshake")]
[JsonDerivedType(typeof(StateUpdateMessage), "stateUpdate")]
[JsonDerivedType(typeof(NoiseEventMessage), "noiseEvent")]
[JsonDerivedType(typeof(PlayerLeftMessage), "playerLeft")]
[JsonDerivedType(typeof(ActionMessage), "action")]
[JsonDerivedType(typeof(JournalLogMessage), "journalLog")]
public interface INetworkMessage
{
    void Execute(INetworkMessageHandler handler);
}
