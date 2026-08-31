using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CosmosColdMusic;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
internal static class AfterCardPlayedPatch
{
    // Hook.AfterCardPlayed(ICombatState, PlayerChoiceContext, CardPlay)
    // uses index 2 for the play so this patch is not tied to the parameter name.
    private static void Postfix(CardPlay __2)
    {
        if (__2.Card is CosmicIndifference)
        {
            // Multiplayer card actions execute on every peer. Do not filter with
            // LocalContext.IsMe: remote players must trigger this client's audio too.
            MusicReplacement.Play(__2.Player.NetId);
        }
    }
}
