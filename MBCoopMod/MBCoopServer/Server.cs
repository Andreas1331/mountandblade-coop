using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MBCoopServer
{
    public class Server
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly TcpListener _serverHandle;

        private int nextClientID = 0;
        private readonly Dictionary<string, Client> _connectedClients = new Dictionary<string, Client>();

        public Server(string ipAddress, int port)
        {
            _ipAddress = IPAddress.Parse(ipAddress);
            _port = port;
            _serverHandle = new TcpListener(_ipAddress, _port);
        }

        public void StartServer()
        {
            // Run the server on a sepearte thread than the caller thread
            Task.Run(() =>
            {
                try
                {
                    // Start the server, and print a message
                    _serverHandle.Start();
                    Console.WriteLine("[MBCoop] Server has successfully been started!");

                    // Infinite loop waiting for connections
                    while (true)
                    {
                        Console.WriteLine("Waiting for a new connection...");

                        // This is a blocking call, until a new connection is eastablished
                        TcpClient client = _serverHandle.AcceptTcpClient();
                        HandleNewConnection(client);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Server exception: {0}", e);
                }
                finally
                {
                    _serverHandle.Stop();
                }

                Console.WriteLine("\nHit enter to continue...");
                Console.ReadKey();
            });
        }

        private void HandleNewConnection(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();

            int i;
            byte[] buffer = new byte[256];
            string username = "";
            // Loop to receive all the data sent by the client.
            while (!stream.DataAvailable) ;
            while (stream.DataAvailable)
            {
                i = stream.Read(buffer, 0, buffer.Length);
                username = Encoding.UTF8.GetString(buffer, 0, i);
                Console.WriteLine($"User {username} has connected to the server.");
            }

            byte[] responseData;
            // Ensure the client isn't connected already
            if (_connectedClients.ContainsKey(username))
            {
                if (stream.CanWrite)
                {
                    // Send back a response.
                    responseData = Packet.ObjectToByteArray<object>(new object[2] { "[MBCoop] You're already connected!", null });
                    stream.Write(responseData, 0, responseData.Length);
                }

                Console.WriteLine($"User {username} was already connected. Connection has been terminated.");
                stream.Close();
                tcpClient.Close();
                return;
            }
            else
            {
                Client client = new Client(nextClientID++, tcpClient, HandleClientPacketReceived);
                if (stream.CanWrite)
                {
                    // Send back a response, and the assigned client ID
                    responseData = Packet.ObjectToByteArray<object>(new object[2] { "[MBCoop] You've successfully connected!",  client.ID});
                    stream.Write(responseData, 0, responseData.Length);
                }
                _connectedClients.Add(username, client);
            }
        }

        public void BroadcastMessage(string message)
        {
            lock (_connectedClients)
            {
                Packet packet = new Packet(Commands.Message, Encoding.UTF8.GetBytes(message));
                foreach(Client cl in _connectedClients.Values)
                {
                    cl.SendPacket(packet);
                }
            }
        }

        private void HandleClientPacketReceived(Packet packet)
        {
            if(packet != null)
            {
                foreach(Client client in _connectedClients.Values)
                {
                    client.SendPacket(packet);
                }
            }
        }
    }
}
