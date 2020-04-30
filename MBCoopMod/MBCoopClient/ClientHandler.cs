using Helpers;
using MBCoopClient.Messages;
using MBCoopLibrary;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary.NetworkData;
using System;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MBCoopClient
{
    public class ClientHandler
    {
        private static ClientHandler _instance;
        public static ClientHandler Instance
        {
            get
            {
                return _instance == null ? (_instance = new ClientHandler()) : _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public Client Client;

        public MobileParty otherClient;

        public void StartConnection()
        {
            Client = new Client(Environment.UserName, OnClientPacketReceived, isHost: (Environment.UserName.Equals("andre")));
            Tuple<bool, object[]> result = Client.ConnectToServer("192.168.0.22", 13000);

            if (result.Item1)
            {
                MessageHandler.SendMessage(result.Item2[0].ToString());
                HandleFirstTimeConnecting();
            }
            else
            {
                // TODO: Provide a keystroke to reattempt a connection
                MessageHandler.SendMessage("[MBCoop] You failed to establish a connection!");
            }
        }

        private void OnClientPacketReceived(Packet packet)
        {
            if(packet != null)
            {
                MessageHandler.SendMessage("Packet received!");
                switch (packet.Command)
                {
                    case Commands.SendPartyDetails:
                        MobilePartyNetworkContainer container = Packet.FromByteArray<MobilePartyNetworkContainer>(packet.Data);
                        otherClient = MBObjectManager.Instance.CreateObject<MobileParty>(container.Name);
                        otherClient.InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), new Vec2(container.PosX, container.PosY), 5);
                        otherClient.Party.Visuals.SetMapIconAsDirty();
                        otherClient.IsLordParty = true;
                        //otherClient.DisableAi();
                        otherClient.Ai.SetDoNotMakeNewDecisions(true);
                        otherClient.Party.AddMembers(MobileParty.MainParty.MemberRoster.ToFlattenedRoster());
                        break;
                    case Commands.SendPosition:
                        break;
                    case Commands.SendPartyGotoPoint:
                        MobilePartyNetworkContainer container2 = Packet.FromByteArray<MobilePartyNetworkContainer>(packet.Data);
                        MessageHandler.SendMessage("Received SendPartyGotoPoint: " + container2.PosX + "," + container2.PosY);
                        otherClient.SetMoveGoToPoint(new Vec2(container2.PosX, container2.PosY));
                        break;
                    case Commands.Message:
                        string msg = Encoding.UTF8.GetString(packet.Data);
                        MessageHandler.SendMessage(msg);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleFirstTimeConnecting()
        {
            // Send data. Determine if I'm the host or a client first.
            // Client needs to send information regarding his party.
            if (Client.IsHost)
            {
                MobileParty party = MobileParty.MainParty;
                if(party != null)
                {
                    string partyName = party.Name.ToString();
                    Vec2 position = party.GetPosition2D;
                    MobilePartyNetworkContainer container = new MobilePartyNetworkContainer(partyName, position.x, position.y);
                    byte[] data = Packet.ObjectToByteArray(container);
                    Packet packet = new Packet(Commands.SendPartyDetails, data);
                    Client.SendPacket(packet);
                    MessageHandler.SendMessage("Sent party to the server..");
                }
            }
        }


    }
}
