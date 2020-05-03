using MBCoopClient.Messages;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace MBCoopClient.PacketTools
{
    public static class PacketHandler
    {
        public delegate void HandlePacketDel(Packet packet);

        public static Dictionary<Commands, HandlePacketDel> PacketMethods = new Dictionary<Commands, HandlePacketDel>()
        {
            { Commands.SendPartyDetails, OnSendPartyDetailsReceived },
            { Commands.SendPartyGotoPoint, OnSendPartyGotoPointReceived },
            { Commands.SendMessage, OnSendMessageReceived },
            { Commands.NewPlayerConnectedID, OnNewPlayerConnectedIDReceived }
        };

        private static void OnSendPartyDetailsReceived(Packet packet)
        {
            MobilePartyNetworkContainer container = Packet.FromByteArray<MobilePartyNetworkContainer>(packet.Data);
            //otherClient = MBObjectManager.Instance.CreateObject<MobileParty>(container.Name);
            //otherClient.InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), new Vec2(container.PosX, container.PosY), 5);
            //otherClient.Party.Visuals.SetMapIconAsDirty();
            //otherClient.IsLordParty = true;
            ////otherClient.DisableAi();
            //otherClient.Ai.SetDoNotMakeNewDecisions(true);
            //otherClient.Party.AddMembers(MobileParty.MainParty.MemberRoster.ToFlattenedRoster());
        }

        private static void OnSendPartyGotoPointReceived(Packet packet)
        {
            MobilePartyNetworkContainer container2 = Packet.FromByteArray<MobilePartyNetworkContainer>(packet.Data);
            MessageHandler.SendMessage("Received SendPartyGotoPoint: " + container2.PosX + "," + container2.PosY);
            //otherClient.SetMoveGoToPoint(new Vec2(container2.PosX, container2.PosY));
        }

        private static void OnSendMessageReceived(Packet packet)
        {
            string msg = Encoding.UTF8.GetString(packet.Data);
            MessageHandler.SendMessage(msg);
        }

        private static void OnNewPlayerConnectedIDReceived(Packet packet)
        {
            // TODO: Do a proper try-parse
            int id = int.Parse(Encoding.UTF8.GetString(packet.Data));
            //otherClients.Add(id);
        }
    }
}
