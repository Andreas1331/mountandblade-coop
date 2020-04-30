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
using System.Collections.Generic;
using MBCoopClient.PacketTools;

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
        public List<int> otherClients = new List<int>();

        private Client _client;
        public Client Client
        {
            get
            {
                return _client == null ? throw new Exception("Client is null!") : _client;
            }
            set
            {
                _client = value;
            }
        }

        public MobileParty otherClient;

        public void StartConnection()
        {
            Client = new Client(Environment.UserName, OnClientPacketReceived, isHost: (Environment.UserName.Equals("andre")));
            Tuple<bool, object[]> result = Client.ConnectToServer("192.168.0.22", 13000);

            if (result.Item1)
            {
                MessageHandler.SendMessage(result.Item2[0].ToString());
                Client.ID = Convert.ToInt32(result.Item2[1]);
                MessageHandler.SendMessage("Granted ID : " + Client.ID);
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
                PacketHandler.HandlePacketDel packetMethod;
                if(PacketHandler.PacketMethods.TryGetValue(packet.Command, out packetMethod))
                {
                    packetMethod.Invoke(packet);
                }
            }
        }

        private void HandleFirstTimeConnecting()
        {
            // Send data. Determine if I'm the host or a client first.
            // Client needs to send information regarding his party.
            // TODO: Delete all the clients Ai
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
