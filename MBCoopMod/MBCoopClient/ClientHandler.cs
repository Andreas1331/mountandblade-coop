using MBCoopClient.Messages;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
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

        public void AttemptPromptForIpaddress()
        {
            // Game hasn't started yet
            if (Campaign.Current == null)
                return;

            if(Client == null)
            {
                // Prompt
                TextInquiryData inqData = new TextInquiryData("Enter server ipaddress", "(Example: 192.168.1.0:9999)", true, true, "Connect", "Cancel", OnConnectPressed, null, false, IsIpaddressValid);
                InformationManager.ShowTextInquiry(inqData, true);
            }
            else
            {
                // Prompt for disconnection
            }
        }

        private void OnConnectPressed(string msg)
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
            // TODO: Use regex to validate format
            return !String.IsNullOrEmpty(msg);
        }

        private void StartConnection(string ip, int port)
        {
            // Prompt
            InquiryData inqData = new InquiryData("Is host?", "", true, true, "Yes", "No", 
                () => 
                {
                    Client = new GameHost(Environment.UserName);
                    Continue(ip, port);
                }, 
                () => 
                {
                    Client = new GameClient(Environment.UserName);
                    Continue(ip, port);
                }, null);
            InformationManager.ShowInquiry(inqData, true);

            // TODO: Determine who's the host differently...
            //if (Environment.UserName.Equals("andre"))
            //{
            //    Client = new GameHost(Environment.UserName);
            //}
            //else
            //{
            //    Client = new GameClient(Environment.UserName);
            //}
            //Tuple<bool, object[]> result = Client.ConnectToServer(ip, port);

            //if (result.Item1)
            //{
            //    Client.ID = Convert.ToInt32(result.Item2[1]);
            //    Client.OnFirstTimeConnecting(result.Item2[0].ToString());
            //}
            //else
            //{
            //    MessageHandler.SendMessage("[MBCoop] You failed to establish a connection!");
            //}
        }

        private void Continue(string ip, int port)
        {
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
