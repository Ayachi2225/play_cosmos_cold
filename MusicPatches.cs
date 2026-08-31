using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace CosmosColdMusic;

// A new run/act track supersedes our replacement track.
[HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.UpdateMusic))]
internal static class RunMusicUpdatePatch
{
    private static void Prefix() => MusicReplacement.Stop();
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
    private static void Postfix(float volume) => MusicReplacement.SetBgmVolume(volume);
}

