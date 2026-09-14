using System.Text.Json.Serialization;

namespace ProJob.Config;

public sealed class GameConfig
{
    [JsonPropertyName("playerName")]
    public string PlayerName { get; set; } = "Bohater";

    [JsonPropertyName("dungeonTheme")]
    public string DungeonTheme { get; set; } = "library";

    [JsonPropertyName("journalSavePath")]
    public string JournalSavePath { get; set; } = ".";
}
