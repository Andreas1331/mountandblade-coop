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
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

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
            { Commands.RequestFirsttimeConnection, OnRequestFirstimeConnection }
        };

        private static void OnRequestFirstimeConnection(Packet packet)
        {
            // TODO: Only send this data to the actual player who requested it .. 
            if (ClientHandler.Instance.Client.IsHost)
            {
                MessageHandler.SendMessage("RequestFirsttimeConnection");
                MobileParty party = MobileParty.MainParty;
                string partyName = party.Name.ToString();
                Vector2 position = new Vector2(party.Position2D.x, party.Position2D.y);
                MobilePartyNetworkContainer container = new MobilePartyNetworkContainer(partyName, position);
                byte[] data = Packet.ObjectToByteArray(container);
                Packet dataPacket = new Packet(Commands.SendPartyDetails, data);
                ClientHandler.Instance.Client.SendPacket(dataPacket);
                MessageHandler.SendMessage("Someone requested data from you!");
            }
        }

        private static void OnSendPartyDetailsReceived(Packet packet)
        {
            MobilePartyNetworkContainer container = Packet.FromByteArray<MobilePartyNetworkContainer>(packet.Data);
            MobileParty newParty = MBObjectManager.Instance.CreateObject<MobileParty>(container.Name);
            newParty.InitializeMobileParty(new TextObject(container.Name), new TroopRoster(), new TroopRoster(), new Vec2(container.Position.X, container.Position.Y), 5);
            newParty.Party.Visuals.SetMapIconAsDirty();
            newParty.IsLordParty = true;
            //otherClient.DisableAi();
            newParty.Ai.SetDoNotMakeNewDecisions(true);
            newParty.Party.AddMembers(MobileParty.MainParty.MemberRoster.ToFlattenedRoster());
        }

        private static void OnSendPartyGotoPointReceived(Packet packet)
        {
            MobilePartyNetworkContainer mpContainer = Packet.FromByteArray<MobilePartyNetworkContainer>(packet.Data);
            if(mpContainer != null)
            {
                MobileParty party = MobileParty.All.FirstOrDefault(x => x.Name.ToString().Equals(mpContainer.Name));
                // To avoid circular calling, don't invoke SetMove for the mainparty
                if(party != null && !party.IsMainParty)
                {
                    party.SetMoveGoToPoint(new Vec2(mpContainer.Position.X, mpContainer.Position.Y));
                }
            }
        }

        private static void OnSendMessageReceived(Packet packet)
        {
            string msg = Encoding.UTF8.GetString(packet.Data);
            MessageHandler.SendMessage(msg);
        }
    }
}
