using MBCoopLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly List<TcpClient> _connectedClients = new List<TcpClient>();

        public Server(string ipAddress, int port)
        {
            _ipAddress = IPAddress.Parse(ipAddress);
            _port = port;
            _serverHandle = new TcpListener(_ipAddress, _port);
        }

        public void StartServer()
        {
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

                        // Continue to listen for input from the player
                        Task.Run(() =>
                        {
                            while ((true))
                            {
                                try
                                {
                                    if (client == null || !client.Connected)
                                    {
                                        // TODO: Remove the player from the list of players
                                        Console.WriteLine("Client has disconnected!");
                                        return;
                                    }
                                    if (client.GetStream() == null)
                                        break;

                                    if(client.GetStream().CanRead && client.GetStream().DataAvailable)
                                    {
                                        NetworkStream networkStream = client.GetStream();
                                        byte[] dataBytes = new byte[256];

                                        networkStream.Read(dataBytes, 0, dataBytes.Length);
                                        string dataFromClient = Encoding.UTF8.GetString(dataBytes);

                                        Console.WriteLine("From client: " + dataFromClient);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                        });
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

        private void HandleNewConnection(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            _connectedClients.Add(client);

            int i;
            byte[] buffer = new byte[256];
            List<byte> data = new List<byte>();
            // Loop to receive all the data sent by the client.
            while (!stream.DataAvailable) ;
            while (stream.DataAvailable)
            {
                i = stream.Read(buffer, 0, buffer.Length);
                //buffer.ToList().ForEach(x => data.Add(x));
                string msgFromClient = Encoding.UTF8.GetString(buffer, 0, i);
                Console.WriteLine($"Username: {msgFromClient} has connected to the server.");
            }

            if (stream.CanWrite)
            {
                // Send back a response.
                byte[] msg = Encoding.UTF8.GetBytes("[MBCoop] You've successfully connected!");
                stream.Write(msg, 0, msg.Length);
            }
        }

        public void CheckConnections()
        {
            return;
            Task.Run(() =>
            {
                while (true)
                {
                    lock (_connectedClients)
                    {
                        foreach (TcpClient client in _connectedClients)
                        {
                            if (client == null || !client.Connected)
                            {
                                Console.WriteLine("CLIENT HAS DISCONNECTED!");
                            }
                            else
                            {
                                Console.WriteLine("Client is still connected!");
                            }
                        }
                    }
                }
            });
        }

        public void BroadcastMessage(string msg)
        {
            foreach (TcpClient client in _connectedClients)
            {
                Console.WriteLine("Broadcasting to client...");
                NetworkStream stream = client.GetStream();

                // Send the message to the TcpServer
                byte[] usernameBytes = Encoding.UTF8.GetBytes(msg);
                stream.Write(usernameBytes, 0, usernameBytes.Length);
            }
            Console.WriteLine("Sent message: " + msg);
        }
    }
}
