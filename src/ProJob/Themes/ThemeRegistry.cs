using ProJob.Themes.LibraryTheme;
using ProJob.Themes.SteampunkTheme;
using ProJob.Themes.VaultTheme;

namespace ProJob.Themes;

public static class ThemeRegistry
{
    private static readonly Dictionary<string, IDungeonThemeFactory> _themes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["library"] = new LibraryThemeFactory(),
            ["steampunk"] = new SteampunkThemeFactory(),
            ["vault"] = new VaultThemeFactory(),
        };

    public static IDungeonThemeFactory Get(string key)
        => _themes.TryGetValue(key, out var factory) ? factory : new LibraryThemeFactory();

    public static IReadOnlyDictionary<string, IDungeonThemeFactory> All => _themes;
}
