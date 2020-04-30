using HarmonyLib;
using MBCoopClient.Messages;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace MBCoopClient.HarmonyPatches
{
    [HarmonyPatch(typeof(MobileParty))]
    public class MobilePartyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch("SetMoveGoToSettlement")]
        private static void PrefixPatch_SetMoveGotoSettlement(MobileParty __instance, Settlement settlement)
        {
            if (__instance != ClientHandler.Instance.otherClient)
            {
                return;
                MessageHandler.SendMessage("SetMoveGoToSettlement!");
                MobilePartyNetworkContainer container = new MobilePartyNetworkContainer("", settlement.Position2D.x, settlement.Position2D.y);
                Packet packet = new Packet(Commands.SendPartyGotoPoint, Packet.ObjectToByteArray(container));
                if (ClientHandler.Instance.Client != null)
                {
                    ClientHandler.Instance.Client.SendPacket(packet);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetMoveGoToPoint")]
        private static void PrefixPatch_SetMoveGoToPoint(MobileParty __instance, Vec2 point)
        {
        }
    }
}
