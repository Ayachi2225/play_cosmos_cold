namespace CosmosColdMusic;

internal enum SongMode
{
    Original,
    Refrain,
    Stars,
    Rotation,
}

internal readonly record struct SongTrack(string FileName, string DisplayName);

internal static class SongSelection
{
    internal const string OriginalValue = "原版 / Original";
    internal const string RefrainValue = "仅副歌 / Refrain";
    internal const string StarsValue = "群星版 / Stars";
    internal const string RotationValue = "轮换 / Rotation";
    internal const string DefaultValue = OriginalValue;

    internal static readonly string[] ModeValues =
    {
        OriginalValue,
        RefrainValue,
        StarsValue,
        RotationValue,
    };

    internal static readonly SongTrack[] Tracks =
    {
        new("original.mp3", OriginalValue),
        new("refrain.mp3", RefrainValue),
        new("stars.mp3", StarsValue),
    };

    private static readonly object Sync = new();
    private static SongMode _mode = SongMode.Original;
    private static int _rotationIndex;

    internal static SongMode Mode
    {
        get
        {
            lock (Sync)
            {
                return _mode;
            }
        }
    }

    internal static string CurrentValue
    {
        get
        {
            lock (Sync)
            {
                return ModeValues[(int)_mode];
            }
        }
    }

    internal static SongMode SetMode(string? value)
    {
        SongMode newMode = value switch
        {
            RefrainValue => SongMode.Refrain,
            StarsValue => SongMode.Stars,
            RotationValue => SongMode.Rotation,
            _ => SongMode.Original,
        };

        return SetMode(newMode);
    }

    internal static SongMode SetMode(SongMode newMode)
    {
        lock (Sync)
        {
            if (_mode != newMode)
            {
                _mode = newMode;
                _rotationIndex = 0;
            }

            return _mode;
        }
    }

    internal static SongTrack TakeNextTrack()
    {
        lock (Sync)
        {
            return _mode switch
            {
                SongMode.Refrain => Tracks[1],
                SongMode.Stars => Tracks[2],
                SongMode.Rotation => TakeNextRotationTrack(),
                _ => Tracks[0],
            };
        }
    }

    private static SongTrack TakeNextRotationTrack()
    {
        SongTrack track = Tracks[_rotationIndex];
        _rotationIndex = (_rotationIndex + 1) % Tracks.Length;
        return track;
    }
}
