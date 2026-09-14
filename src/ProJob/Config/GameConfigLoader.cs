using System.Text.Json;

namespace ProJob.Config;

public static class GameConfigLoader
{
    private static readonly JsonSerializerOptions _options = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static GameConfig Load(string path)
    {
        if (!File.Exists(path))
            return new GameConfig();

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameConfig>(json, _options) ?? new GameConfig();
    }
}
