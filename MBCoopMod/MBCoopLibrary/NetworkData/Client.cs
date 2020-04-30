using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MBCoopLibrary.NetworkData
{
    public class Client
    {
        public readonly int ID;
        public TcpClient TcpClientHandle { get; private set; }
        private readonly string _username;
        public readonly bool IsHost = false;

        public delegate void OnPacketReceived(Packet packet);
        private OnPacketReceived _packetReceived; 

        // Used by the client
        public Client(string username, OnPacketReceived packetReceived, bool isHost)
        {
            _username = username;
            _packetReceived = packetReceived;
            IsHost = isHost;
        }

        // Used by the server
        public Client(int id, TcpClient tcpClient, OnPacketReceived packetReceived)
        {
            TcpClientHandle = tcpClient;
            _packetReceived = packetReceived;
            ListenForPackets();
        }

        // TODO: Refactor so it's reading the response from the server properly + make it async
        public Tuple<bool, object[]> ConnectToServer(string ipAddress, int port)
        {
            // Create the TcpClient
            TcpClientHandle = new TcpClient(ipAddress, port);
            NetworkStream stream = TcpClientHandle.GetStream();

            // Send the message to the TcpServer
            byte[] usernameBytes = Encoding.UTF8.GetBytes(_username);
            stream.Write(usernameBytes, 0, usernameBytes.Length);

            // Receive the TcpServer response
            // Buffer to store the response bytes
            byte[] data = new byte[256];

            // String to store the response UTF8 representation
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
                        //responseData = Encoding.UTF8.GetString(data, 0, bytes);

                        object[] response = Packet.FromByteArray<object[]>(data);
                        ListenForPackets();
                        handle = !handle;

                        return new Tuple<bool, object[]>(true, response);
                    }
                }
            }

            // We failed to establish a connection
            return null;
        }

        private bool IsDisconnected()
        {
            try
            {
                return TcpClientHandle == null || (TcpClientHandle.Client.Poll(10 * 1000, SelectMode.SelectRead) && (TcpClientHandle.Client.Available == 0));
            }
            catch (SocketException se)
            {
                // Handle exception
                return true;
            }
        }

        public void SendPacket(Packet packet)
        {
            try
            {
                NetworkStream stream = TcpClientHandle.GetStream();

                // Convert JSON to dataBuffer and its length to a 16 bit unsigned integer buffer
                byte[] dataBuffer = Encoding.UTF8.GetBytes(packet.ToJson());
                byte[] lengthBuffer = BitConverter.GetBytes(Convert.ToUInt16(dataBuffer.Length));

                // Join the buffers
                byte[] msgBuffer = new byte[lengthBuffer.Length + dataBuffer.Length];
                lengthBuffer.CopyTo(msgBuffer, 0);
                dataBuffer.CopyTo(msgBuffer, lengthBuffer.Length);

                // Send the packet
                stream.Write(msgBuffer, 0, msgBuffer.Length);
                Debug.WriteLine("Sent package!");
            }
            catch (Exception e)
            {
                // TODO: Handle the exception 
                // There was an issue in sending
                Debug.WriteLine("Reason: {0}", e.Message);
            }
        }

        private void ListenForPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Packet packet = null;
                    try
                    {
                        // First check there is data available
                        if (TcpClientHandle.Available == 0)
                            continue;

                        NetworkStream stream = TcpClientHandle.GetStream();

                        // There must be some incoming data, the first two bytes are the size of the Packet
                        byte[] lengthBuffer = new byte[2];
                        int readLength = 0;
                        // Keep looping until we've read the first 2 bytes, so we can determine the packet size
                        // TODO: Add a timeout for the while loop incase we never read the 2 bytes
                        do
                        {
                            int oldReadLength = readLength;
                            readLength = stream.Read(lengthBuffer, oldReadLength, Configuration.Instance.MAX_BYTE_LENGTH - oldReadLength);
                            readLength = oldReadLength + readLength;
                        } while (readLength < Configuration.Instance.MAX_BYTE_LENGTH);

                        ushort packetByteSize = BitConverter.ToUInt16(lengthBuffer, 0);
                        // Now read that many bytes from what's left in the stream, it must be the Packet
                        byte[] packetBuffer = new byte[packetByteSize];
                        readLength = 0;
                        do
                        {
                            int oldReadLength = readLength;
                            readLength = stream.Read(packetBuffer, oldReadLength, packetByteSize - oldReadLength);
                            readLength = oldReadLength + readLength;
                        } while (readLength < packetByteSize);

                        // Convert it into a packet datatype
                        string jsonString = Encoding.UTF8.GetString(packetBuffer);
                        packet = Packet.FromJson(jsonString);

                        _packetReceived(packet);
                    }
                    catch (Exception e)
                    {
                        // There was an issue in receiving
                        Debug.WriteLine("Receiving exception: {0}", e.Message);
                    }
                }
            });
        }
    }
}
