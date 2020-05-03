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
using MBCoopClient.Network;

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

        private Client _client;
        public Client Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
            }
        }

        public void StartConnection()
        {
            if(Client != null)
            {
                MessageHandler.SendMessage("You're already connected to a server!");
                return;
            }

            if (Environment.UserName.Equals("andre"))
            {
                Client = new GameHost(Environment.UserName);
            }
            else
            {
                Client = new GameClient(Environment.UserName);
            }
            Tuple<bool, object[]> result = Client.ConnectToServer("192.168.0.22", 13000);

            if (result.Item1)
            {
                Client.ID = Convert.ToInt32(result.Item2[1]);
                Client.OnFirstTimeConnecting(result.Item2[0].ToString());
            }
            else
            {
                // TODO: Provide a keystroke to reattempt a connection
                MessageHandler.SendMessage("[MBCoop] You failed to establish a connection!");
            }
        }

        // BaseClient is the client who received a Packet from the server
        private void OnClientPacketReceived(BaseClient baseClient, Packet packet)
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
    }
}
