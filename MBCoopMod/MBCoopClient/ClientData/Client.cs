using MBCoopClient.Messages;
using MBCoopLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace MBCoopClient.ClientData
{
    public class Client
    {
        private TcpClient _client;
        private readonly string _ipAddress;
        private readonly string _username;
        private readonly int _port;

        private bool _isConnected => !(_client == null || !_client.Connected || _client.GetStream() == null);

        public Client(string ipAddress, int port, string username)
        {
            _ipAddress = ipAddress;
            _port = port;
            _username = username;
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            // Create the TcpClient
            _client = new TcpClient(_ipAddress, _port);
            NetworkStream stream = _client.GetStream();

            // Send the message to the TcpServer
            byte[] usernameBytes = Encoding.UTF8.GetBytes(_username);
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

            ListenToServer();
        }

        private void ListenToServer()
        {
            Task.Run(() =>
            {
                while (_isConnected)
                {
                    if (_client.GetStream().CanRead && _client.GetStream().DataAvailable)
                    {
                        NetworkStream networkStream = _client.GetStream();
                        byte[] dataBytes = new byte[256];

                        // Load all the sent data into memory
                        byte[] data = new byte[1024];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int numBytesRead = networkStream.Read(data, 0, data.Length);

                            do
                            {
                                ms.Write(data, 0, numBytesRead);
                                if (networkStream.DataAvailable)
                                {
                                    numBytesRead = networkStream.Read(data, 0, data.Length);
                                    if (!networkStream.DataAvailable)
                                    {
                                        ms.Write(data, 0, numBytesRead);
                                    }
                                }
                            }
                            while (networkStream.DataAvailable);

                            string msg = Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
                            msg = msg.Substring(0, msg.IndexOf("$"));

                            List<Settlement>.Enumerator settlement = Settlement.All.GetEnumerator();
                            while (settlement.MoveNext())
                            {
                                if (settlement.Current.Name.ToString().ToLower() == msg.ToLower())
                                {
                                    MessageHandler.SendMessage("Found settlement: " + settlement.Current.Name);
                                    MobileParty.MainParty.SetMoveGoToSettlement(settlement.Current);
                                    break;
                                }
                            }

                            MessageHandler.SendMessage("Received: " + msg);
                        }
                    }
                }
                MessageHandler.SendMessage("[MBCoop] You've lost connection to the server!");
            });
        }

        public void SendGotoPosition(Vector3 position)
        {
            SendData(position);
        }

        protected void SendData<T>(T data)
        {
            NetworkStream stream = _client.GetStream();
            // Send the message to the TcpServer
            byte[] dataInBytes = ObjectConverter<T>.ObjectToByteArray(data);
            stream.Write(dataInBytes, 0, dataInBytes.Length);
        }

        public void SendMessage(string msg)
        {
            NetworkStream stream = _client.GetStream();
            // Send the message to the TcpServer
            byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
            MessageHandler.SendMessage("Bytes sent: " + messageBytes.Length);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
    }
}
