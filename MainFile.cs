using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace CosmosColdMusic;

[ModInitializer(nameof(Initialize))]
public static class MainFile
{
    public const string ModId = "CosmosColdMusic";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        var harmony = new Harmony(ModId);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        BaseLibSettings.Initialize();
        MusicReplacement.Initialize();
    }
}
