using HarmonyLib;
using MBCoopClient.Messages;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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
            // The host will send information about a new party getting created
            if (ClientHandler.Instance.Client.IsHost)
            {
                // TODO: Send new party details ...
            }
            else
            {

            }
            // The client will keep track of newly created parties incase they were added by his own game
            if (__instance.Name.Equals("{=!}unnamedMobileParty"))
            {
                MessageHandler.SendMessage("New MobileParty: " + __instance.Name.Length);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetMoveGoToSettlement")]
        private static void PrefixPatch_SetMoveGotoSettlement(MobileParty __instance, Settlement settlement)
        {
            //if (ClientHandler.Instance.Client.IsHost)
            //{
            //    // Send everything
            //}
            //else
            //{
            //    // This is a client, so only send his own movement
            //    if (__instance.IsMainParty)
            //    {
            //        string settlementName = settlement.GetName().ToString();
            //        byte[] data = Encoding.UTF8.GetBytes(settlementName);
            //        Packet packet = new Packet(Commands.SendPartyGotoSettlement, data);
            //        ClientHandler.Instance.Client.SendPacket(packet);
            //    }
            //}
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetMoveGoToPoint")]
        private static void PrefixPatch_SetMoveGoToPoint(MobileParty __instance, Vec2 point)
        {
            //if (ClientHandler.Instance.Client.IsHost)
            //{
            //    // Send everything
            //}
            //else
            //{
            //    // This is a client, so only send his own movement
            //    if (__instance.IsMainParty)
            //    {
            //    }
            //}
        }
    }
}
