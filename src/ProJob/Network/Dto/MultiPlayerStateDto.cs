namespace ProJob.Network.Dto;

public sealed class MultiPlayerStateDto : GameStateDto
{
    public int YourPlayerId { get; set; }
    public List<OtherPlayerDto> OtherPlayers { get; set; } = [];
}
