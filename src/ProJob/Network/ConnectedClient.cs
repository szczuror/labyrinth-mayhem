using System.Net.Sockets;
using System.Text;

namespace ProJob.Network;

public sealed class ConnectedClient
{
    public int PlayerId { get; }
    public TcpClient Tcp { get; }
    public StreamReader Reader { get; }
    public StreamWriter Writer { get; }
    public ProJob.Player.Player Player { get; }
    public SemaphoreSlim SendLock { get; } = new(1, 1);

    public ConnectedClient(int playerId, TcpClient tcp, ProJob.Player.Player player)
    {
        PlayerId = playerId;
        Tcp = tcp;
        Player = player;

        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        Reader = new StreamReader(tcp.GetStream(), encoding, leaveOpen: true);
        Writer = new StreamWriter(tcp.GetStream(), encoding, leaveOpen: true) { AutoFlush = false };
    }
}
