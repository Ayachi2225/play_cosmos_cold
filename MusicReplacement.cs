using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Saves;

namespace CosmosColdMusic;

internal static class MusicReplacement
{
    private static readonly string AudioDirectory = Path.Combine(GetModDirectory(), "audio");

    private static AudioStreamPlayer? _player;
    private static float _masterVolume = 0.5f;
    private static float _bgmVolume = 0.5f;

    internal static void Initialize()
    {
        foreach (SongTrack track in SongSelection.Tracks)
        {
            string path = GetAudioPath(track);
            if (!File.Exists(path))
            {
                GD.PrintErr($"[{MainFile.ModId}] Packaged audio file not found: {path}");
            }
        }

        RefreshSavedVolumes();

        GD.Print($"[{MainFile.ModId}] Initialized. Packaged audio directory: {AudioDirectory}");
    }

    internal static void Play(ulong sourcePlayerNetId)
    {
        try
        {
            SongTrack track = SongSelection.TakeNextTrack();
            string audioPath = GetAudioPath(track);

            if (!File.Exists(audioPath))
            {
                GD.PrintErr($"[{MainFile.ModId}] Cosmic Indifference was played, but the packaged audio file is missing: {audioPath}");
                return;
            }

            if (Engine.GetMainLoop() is not SceneTree tree || tree.Root is null)
            {
                GD.PrintErr($"[{MainFile.ModId}] Could not access the Godot scene tree.");
                return;
            }

            // Playing the card again restarts the track from the beginning.
            Stop();

            // Stop both run-specific and global FMOD music before starting the MP3.
            NRunMusicController.Instance?.StopMusic();
            NAudioManager.Instance?.StopMusic();

            // Mod initializers run early; reread settings now that the game is fully loaded.
            RefreshSavedVolumes();

            AudioStreamMP3 stream = AudioStreamMP3.LoadFromFile(audioPath);
            stream.Loop = true;

            _player = new AudioStreamPlayer
            {
                Name = MainFile.ModId,
                Stream = stream,
                PitchScale = 1f,
            };

            RefreshVolume();
            tree.Root.AddChild(_player);
            _player.Play();

            GD.Print($"[{MainFile.ModId}] Cosmic Indifference played by player {sourcePlayerNetId}; playing {track.DisplayName} ({SongSelection.Mode}).");
        }
        catch (Exception exception)
        {
            GD.PrintErr($"[{MainFile.ModId}] Could not play replacement music: {exception}");
            Stop();
        }
    }

    internal static void Stop()
    {
        AudioStreamPlayer? player = _player;
        _player = null;

        if (player is null)
        {
            return;
        }

        try
        {
            player.Stop();
            player.QueueFree();
        }
        catch (ObjectDisposedException)
        {
            // The scene tree may already have disposed the player during shutdown.
        }
    }

    internal static void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp(volume, 0f, 1f);
        RefreshVolume();
    }

    internal static void SetBgmVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp(volume, 0f, 1f);
        RefreshVolume();
    }

    private static void RefreshVolume()
    {
        if (_player is null)
        {
            return;
        }

        // The game squares each slider value before sending it to FMOD.
        // Applying both squared values matches the game's response curve.
        float master = _masterVolume * _masterVolume;
        float bgm = _bgmVolume * _bgmVolume;
        _player.VolumeLinear = master * bgm;
    }

    private static void RefreshSavedVolumes()
    {
        try
        {
            SettingsSave settings = SaveManager.Instance.SettingsSave;
            _masterVolume = settings.VolumeMaster;
            _bgmVolume = settings.VolumeBgm;
        }
        catch (Exception exception)
        {
            GD.PrintErr($"[{MainFile.ModId}] Could not read volume settings; using cached values: {exception.Message}");
        }
    }

    private static string GetAudioPath(SongTrack track)
    {
        return Path.Combine(AudioDirectory, track.FileName);
    }

    private static string GetModDirectory()
    {
        string assemblyPath = Assembly.GetExecutingAssembly().Location;
        string? directory = Path.GetDirectoryName(assemblyPath);
        return string.IsNullOrEmpty(directory) ? AppContext.BaseDirectory : directory;
    }
}
