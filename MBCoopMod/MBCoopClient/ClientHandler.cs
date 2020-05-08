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
using TaleWorlds.Diamond;

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

        public void AttemptPromptForIpaddress()
        {
            // Game hasn't started yet
            if (Campaign.Current == null)
                return;

            if(Client == null)
            {
                // Prompt
                TextInquiryData inqData = new TextInquiryData("Enter server ipaddress", "(Example: 192.168.1.0:9999)", true, true, "Connect", "Cancel", OnConnect, null, false, IsIpaddressValid);
                InformationManager.ShowTextInquiry(inqData, true);
            }
            else
            {
                // Prompt for disconnection
            }
        }

        private void OnConnect(string msg)
        {
            string ip = msg.Substring(0, msg.IndexOf(':'));
            int port;
            if(int.TryParse(msg.Substring(msg.IndexOf(':') + 1, (msg.Length - (msg.IndexOf(':') + 1))), out port))
            {
                StartConnection(ip, port);
            }
            else
            {
                MessageHandler.SendMessage("Invalid ipaddress entered, please try again.");
            }
        }

        private bool IsIpaddressValid(string msg)
        {
            return !String.IsNullOrEmpty(msg);
        }

        private void StartConnection(string ip, int port)
        {
            // TODO: Determine who's the host differently...
            if (Environment.UserName.Equals("andre"))
            {
                Client = new GameHost(Environment.UserName);
            }
            else
            {
                Client = new GameClient(Environment.UserName);
            }
            Tuple<bool, object[]> result = Client.ConnectToServer(ip, port);

            if (result.Item1)
            {
                Client.ID = Convert.ToInt32(result.Item2[1]);
                Client.OnFirstTimeConnecting(result.Item2[0].ToString());
            }
            else
            {
                MessageHandler.SendMessage("[MBCoop] You failed to establish a connection!");
            }
        }
    }
}
