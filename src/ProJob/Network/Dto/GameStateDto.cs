namespace ProJob.Network.Dto;

public class GameStateDto
{
    public BoardDto Board { get; set; } = new();
    public PlayerDto Player { get; set; } = new();
    public string CurrentMode { get; set; } = "Exploration";
    public string PlayerName { get; set; } = string.Empty;
    public string WelcomeMessage { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public EnemyDto? CurrentEnemy { get; set; }
    public int CurrentEnemyRow { get; set; }
    public int CurrentEnemyCol { get; set; }
    public int SelectedInventoryIndex { get; set; }
}
