using MBCoopClient.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace MBCoopClient.Client_tools
{
    public class Client
    {
        private readonly TcpClient _client;
        private readonly string _ipAddress;
        private readonly int _port;

        public Client(string ipAddress, int port, string username)
        {
            // Create the TcpClient
            _ipAddress = ipAddress;
            _port = port;
            _client = new TcpClient(_ipAddress, _port);

            ConnectToServer(username);
            ListenForServer();
        }

        bool shownMsg = false;
        private void ListenForServer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if ((_client == null || !_client.Connected) && !shownMsg)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("You're no longer connected!"));
                        shownMsg = !shownMsg;
                        return;
                    }
                    if (_client.GetStream() == null)
                        break;

                    if (_client.GetStream().CanRead && _client.GetStream().DataAvailable)
                    {
                        NetworkStream networkStream = _client.GetStream();
                        byte[] dataBytes = new byte[256];

                        networkStream.Read(dataBytes, 0, dataBytes.Length);
                        string dataFromClient = Encoding.UTF8.GetString(dataBytes);

                        InformationManager.DisplayMessage(new InformationMessage("From server: " + dataFromClient));
                    }
                }
            });
        }

        private void ConnectToServer(string username)
        {
            NetworkStream stream = _client.GetStream();

            // Send the message to the TcpServer
            byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
            stream.Write(usernameBytes, 0, usernameBytes.Length);

            // Receive the TcpServer response
            // Buffer to store the response bytes
            byte[] data = new byte[256];

            // String to store the response ASCII representation
            string responseData;
            // Read the first batch of the TcpServer response bytes
            if (stream.CanRead)
            {
                bool handle = true;
                while (handle)
                {
                    if (stream.DataAvailable)
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        responseData = Encoding.UTF8.GetString(data, 0, bytes);
                        MessageHandler.SendMessage(responseData);
                        handle = !handle;
                    }
                }
            }

            MessageHandler.SendMessage("Done!");
        }

        public void SendMsg(string msg)
        {
            NetworkStream stream = _client.GetStream();

            // Send the message to the TcpServer
            byte[] usernameBytes = Encoding.UTF8.GetBytes(msg);
            stream.Write(usernameBytes, 0, usernameBytes.Length);
        }
    }
}
