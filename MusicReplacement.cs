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
    private static bool _settingGameBgmInternally;
    private static bool _vanillaBgmMuted;

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
            MuteVanillaBgm();

            GD.Print($"[{MainFile.ModId}] Cosmic Indifference played by player {sourcePlayerNetId}; playing {track.DisplayName} ({SongSelection.Mode}).");
        }
        catch (Exception exception)
        {
            GD.PrintErr($"[{MainFile.ModId}] Could not play replacement music: {exception}");
            Stop();
        }
    }

    internal static void Stop(string? reason = null)
    {
        AudioStreamPlayer? player = _player;
        _player = null;
        bool wasPlaying = player is not null;

        if (player is not null)
        {
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

        RestoreVanillaBgm();

        if (wasPlaying && !string.IsNullOrEmpty(reason))
        {
            GD.Print($"[{MainFile.ModId}] Stopped replacement music ({reason}); restored vanilla BGM.");
        }
    }

    internal static void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp(volume, 0f, 1f);
        RefreshVolume();
    }

    internal static void FilterGameBgmVolume(ref float volume)
    {
        if (_settingGameBgmInternally)
        {
            return;
        }

        _bgmVolume = Mathf.Clamp(volume, 0f, 1f);
        RefreshVolume();

        if (_player is not null)
        {
            volume = 0f;
            _vanillaBgmMuted = true;
        }
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

    private static void MuteVanillaBgm()
    {
        if (_vanillaBgmMuted)
        {
            return;
        }

        NAudioManager? audioManager = NAudioManager.Instance;
        if (audioManager is null)
        {
            return;
        }

        try
        {
            _settingGameBgmInternally = true;
            audioManager.SetBgmVol(0f);
            _vanillaBgmMuted = true;
        }
        catch (Exception exception)
        {
            GD.PrintErr($"[{MainFile.ModId}] Could not mute vanilla BGM: {exception.Message}");
        }
        finally
        {
            _settingGameBgmInternally = false;
        }
    }

    private static void RestoreVanillaBgm()
    {
        if (!_vanillaBgmMuted)
        {
            return;
        }

        _vanillaBgmMuted = false;
        NAudioManager? audioManager = NAudioManager.Instance;
        if (audioManager is null)
        {
            return;
        }

        try
        {
            _settingGameBgmInternally = true;
            audioManager.SetBgmVol(_bgmVolume);
        }
        catch (Exception exception)
        {
            GD.PrintErr($"[{MainFile.ModId}] Could not restore vanilla BGM volume: {exception.Message}");
        }
        finally
        {
            _settingGameBgmInternally = false;
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
