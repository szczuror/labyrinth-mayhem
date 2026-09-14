using ProJob.Network.Dto;

namespace ProJob.Network.Messages;

public sealed record HandshakeMessage(int YourPlayerId, MultiPlayerStateDto State) : INetworkMessage
{
    public void Execute(INetworkMessageHandler h) => h.Handle(this);
}

public sealed record StateUpdateMessage(MultiPlayerStateDto State) : INetworkMessage
{
    public void Execute(INetworkMessageHandler h) => h.Handle(this);
}

public sealed record NoiseEventMessage(int SourceRow, int SourceCol, int Range, int Distance) : INetworkMessage
{
    public void Execute(INetworkMessageHandler h) => h.Handle(this);
}

public sealed record PlayerLeftMessage(int PlayerId) : INetworkMessage
{
    public void Execute(INetworkMessageHandler h) => h.Handle(this);
}

public sealed record ActionMessage(string Action) : INetworkMessage
{
    public void Execute(INetworkMessageHandler h) => h.Handle(this);
}

public sealed record JournalLogMessage(string Text) : INetworkMessage
{
    public void Execute(INetworkMessageHandler h) => h.Handle(this);
}
