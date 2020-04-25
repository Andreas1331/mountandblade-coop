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
                    Console.WriteLine("Waiting for a new connection...");

                    // Infinite loop waiting for connections
                    while (true)
                    {
                        // This is a blocking call, until a new connection is eastablished
                        TcpClient client = _serverHandle.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();
                        _connectedClients.Add(client);

                        Console.WriteLine("New client connected!");
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
                            Console.WriteLine($"Username: {msgFromClient}");
                        }

                        if (stream.CanWrite)
                        {
                            byte[] msg = Encoding.UTF8.GetBytes("[MBCoop] You've successfully connected!");
                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                        }

                        // Finally close the connection to the client again as we're done
                        //client.Close();
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

        public void CheckConnections()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    lock (_connectedClients)
                    {
                        foreach (TcpClient client in _connectedClients)
                        {
                            if (!client.Connected)
                            {
                                Console.WriteLine("CLIENT HAS DISCONNECTED!");
                            }
                        }
                    }
                }
            });
        }

        public void BroadcastMessage(string msg)
        {
            foreach(TcpClient client in _connectedClients)
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] byteMsg = Encoding.UTF8.GetBytes(msg);
                    if (stream.CanWrite)
                    {
                        stream.Write(byteMsg, 0, byteMsg.Length);
                        stream.Flush();
                    }
                    else
                    {
                        Console.WriteLine("Unable to write to stream!");
                    }
                }
                else
                {
                    Console.WriteLine("Client is not connected!");
                }
            }
        }
    }
}
