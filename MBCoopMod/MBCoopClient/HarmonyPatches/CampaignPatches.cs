using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace MBCoopClient.HarmonyPatches
{
    [HarmonyPatch(typeof(Campaign))]
    public class CampaignPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetTimeSpeed")]
        private static void PostfixPatch_SetTimeSpeed(Campaign __instance, int speed)
        {
            __instance.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;
        }
    }
}
