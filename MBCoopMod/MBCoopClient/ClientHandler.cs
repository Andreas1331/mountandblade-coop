using MBCoopClient.Messages;
using MBCoopLibrary.NetworkData;
using System;
using System.Text;

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

        public void StartConnection()
        {
            Client = new Client(Environment.UserName, HandleClientPacket);
            Client.ConnectToServer("192.168.0.22", 13000);
        }

        private void HandleClientPacket(Packet packet)
        {
            if(packet != null)
            {
                MessageHandler.SendMessage("Packet received!");
                MessageHandler.SendMessage("Command: " + packet.Command + " | Data: " + Encoding.UTF8.GetString(packet.Data));
            }
        }
    }
}
