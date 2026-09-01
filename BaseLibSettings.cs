using BaseLib.Config;

namespace CosmosColdMusic;

public enum 歌曲版本
{
    原版,
    仅副歌,
    群星版,
    轮换,
}

internal sealed class CosmosColdConfig : SimpleModConfig
{
    public static 歌曲版本 歌曲选项 { get; set; } = 歌曲版本.原版;
}

internal static class BaseLibSettings
{
    private static CosmosColdConfig? _config;

    internal static void Initialize()
    {
        _config = new CosmosColdConfig();
        ApplySongMode();
        _config.ConfigChanged += (_, _) => ApplySongMode();
        _config.OnConfigReloaded += ApplySongMode;
        ModConfigRegistry.Register(MainFile.ModId, _config);

        MainFile.Logger.Info(
            $"Registered BaseLib mod settings. Local song mode: {SongSelection.Mode}.",
            1);
    }

    private static void ApplySongMode()
    {
        SongMode mode = CosmosColdConfig.歌曲选项 switch
        {
            歌曲版本.仅副歌 => SongMode.Refrain,
            歌曲版本.群星版 => SongMode.Stars,
            歌曲版本.轮换 => SongMode.Rotation,
            _ => SongMode.Original,
        };

        SongSelection.SetMode(mode);
        MainFile.Logger.Info($"Local song mode changed to {mode}; applies on the next play.", 1);
    }
}
