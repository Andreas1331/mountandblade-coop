using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MBCoopLibrary.NetworkData
{
    public abstract class BaseClient
    {
        public string Username;
        public int ID;
        public TcpClient TcpClientHandle { get; protected set; }
        public bool IsHost = false;

        //private bool IsDisconnected()
        //{
        //    try
        //    {
        //        return TcpClientHandle == null || (TcpClientHandle.Client.Poll(10 * 1000, SelectMode.SelectRead) && (TcpClientHandle.Client.Available == 0));
        //    }
        //    catch (SocketException se)
        //    {
        //        // Handle exception
        //        return true;
        //    }
        //}

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

        protected void ListenForPackets()
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
                        OnPacketReceived(packet);
                        //_packetReceived(this, packet);
                    }
                    catch (Exception e)
                    {
                        // There was an issue in receiving
                        Debug.WriteLine("Receiving exception: {0}", e.Message);
                    }
                }
            });
        }

        protected abstract void OnPacketReceived(Packet packet);
    }
}
