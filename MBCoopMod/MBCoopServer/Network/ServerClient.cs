using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System;
using System.Net.Sockets;

namespace MBCoopServer.Network
{
    public class ServerClient : BaseClient
    {
        private readonly Server server;

        public ServerClient(string username, int id, TcpClient tcpClient, Server server)
        { 
            Username = username;
            ID = id;
            TcpClientHandle = tcpClient;
            this.server = server;
            IsHost = (Program.hostFound ? false : true); //(username == "andre");
            if (Program.hostFound == false)
                Program.hostFound = true;

            ListenForPackets();
        }

        // BaseClient is the client who sent the Packet to the server
        protected override void OnPacketReceived(Packet packet)
        {
            if (packet != null)
            {
                // Handle exclusion
                switch (packet.Command)
                {
                    // The client sent a message, don't send it back to himself
                    case Commands.SendMessage:
                    case Commands.SendPartyDetails:
                        server.SendPacketToClients(packet, this);
                        break;
                    default:
                        server.SendPacketToClients(packet);
                        break;
                }
            }
        }
    }
}
