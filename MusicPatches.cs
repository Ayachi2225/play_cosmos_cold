using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace CosmosColdMusic;

// A new run/act track supersedes our replacement track.
[HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.UpdateMusic))]
internal static class RunMusicUpdatePatch
{
    private static void Prefix() => MusicReplacement.Stop();
}

// Every room entry updates the run's Progress track, even when the act's
// background event itself does not change. Stop our MP3 before that transition
// so rest sites, shops, events, and combats regain their vanilla music.
[HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.UpdateTrack), new Type[] { })]
internal static class RunMusicTrackPatch
{
    private static void Prefix() => MusicReplacement.Stop("room music transition");
}

// Special-event music also supersedes our replacement track.
[HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.PlayCustomMusic))]
internal static class RunCustomMusicPatch
{
    private static void Prefix() => MusicReplacement.Stop();
}

[HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.StopMusic))]
internal static class RunMusicStopPatch
{
    private static void Prefix() => MusicReplacement.Stop();
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayMusic))]
internal static class GlobalMusicPlayPatch
{
    private static void Prefix() => MusicReplacement.Stop();
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.StopMusic))]
internal static class GlobalMusicStopPatch
{
    private static void Prefix() => MusicReplacement.Stop();
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.SetMasterVol))]
internal static class MasterVolumePatch
{
    private static void Postfix(float volume) => MusicReplacement.SetMasterVolume(volume);
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.SetBgmVol))]
internal static class BgmVolumePatch
{
    private static void Prefix(ref float volume) => MusicReplacement.FilterGameBgmVolume(ref volume);
}
