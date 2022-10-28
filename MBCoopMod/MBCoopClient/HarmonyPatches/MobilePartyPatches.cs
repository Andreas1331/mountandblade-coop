using HarmonyLib;
using TaleWorlds.Library;

namespace MBCoopClient.HarmonyPatches
{
    [HarmonyPatch(typeof(MobileParty))]
    public class MobilePartyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MobileParty), MethodType.Constructor)]
        private static void PostfixPatch_Constructor(MobileParty __instance)
        {
            ClientHandler.Instance.Client?.OnNewMobilePartyInit(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetMoveGoToSettlement")]
        private static void PrefixPatch_SetMoveGotoSettlement(MobileParty __instance, Settlement settlement)
        {
            ClientHandler.Instance.Client?.OnSetMoveGotoSettlement(__instance, settlement);
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetMoveGoToPoint")]
        private static void PrefixPatch_SetMoveGoToPoint(MobileParty __instance, Vec2 point)
        {
            ClientHandler.Instance.Client?.OnSetMoveGotoPoint(__instance, point);
        }
    }
}


