using MBCoopLibrary;
using System;
using System.Collections.Generic;
using System.IO;
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

                        // Continue to listen for input from the player, but on a new thread 
                        Task.Run(() =>
                        {
                            while (!(client == null || !client.Connected || client.GetStream() == null))
                            {
                                try
                                {
                                    if(client.GetStream().CanRead && client.GetStream().DataAvailable)
                                    {
                                        // Load all the sent data into memory
                                        byte[] data = new byte[1024];
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            NetworkStream networkStream = client.GetStream();
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

                                            Console.WriteLine($"Received data ({ms.ToArray().Length} bytes)");

                                            // Send the information to all the other clients
                                            SendDataToClients(ms.ToArray());
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            // TODO: Remove the player from the list of players
                            Console.WriteLine("Client has disconnected!");
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
            string username;
            // Loop to receive all the data sent by the client.
            while (!stream.DataAvailable) ;
            while (stream.DataAvailable)
            {
                i = stream.Read(buffer, 0, buffer.Length);
                username = Encoding.UTF8.GetString(buffer, 0, i);
                Console.WriteLine($"User {username} has connected to the server.");
            }

            if (stream.CanWrite)
            {
                // Send back a response.
                byte[] msg = Encoding.UTF8.GetBytes("[MBCoop] You've successfully connected!");
                stream.Write(msg, 0, msg.Length);
            }
        }

        public void SendDataToClients(byte[] data)
        {
            lock (_connectedClients)
            {
                foreach (TcpClient client in _connectedClients)
                {
                    NetworkStream stream = client.GetStream();

                    // Send the message to the TcpServer
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        public void BroadcastMessage(string msg)
        {
            lock (_connectedClients)
            {
                foreach (TcpClient client in _connectedClients)
                {
                    NetworkStream stream = client.GetStream();

                    // Send the message to the TcpServer
                    byte[] msgBytes = Encoding.UTF8.GetBytes(msg + "$");
                    stream.Write(msgBytes, 0, msgBytes.Length);
                }
            }
            Console.WriteLine("Sent message: " + msg);
        }
    }
}
