using MBCoopClient.Messages;
using MBCoopClient.PacketTools;
using MBCoopLibrary.NetworkData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace MBCoopClient.Network
{
    public abstract class Client : BaseClient
    {
        protected readonly Chat chat;
        protected event Action<float> OnTickEvent;

        // Used by the client
        public Client(string username, bool isHost)
        {
            Username = username;
            IsHost = isHost;
            chat = new Chat(this);

            RegisterEvents();
        }

        // TODO: Refactor so it's reading the response from the server properly + make it async
        public Tuple<bool, object[]> ConnectToServer(string ipAddress, int port)
        {
            // Create the TcpClient
            TcpClientHandle = new TcpClient(ipAddress, port);
            NetworkStream stream = TcpClientHandle.GetStream();

            // Send the message to the TcpServer
            byte[] usernameBytes = Encoding.UTF8.GetBytes(Username);
            stream.Write(usernameBytes, 0, usernameBytes.Length);

            // Receive the TcpServer response
            // Buffer to store the response bytes
            byte[] data = new byte[256];

            // Read the first batch of the TcpServer response bytes
            if (stream.CanRead)
            {
                bool handle = true;
                while (handle)
                {
                    if (stream.DataAvailable)
                    {
                        stream.Read(data, 0, data.Length);
                        object[] response = Packet.FromByteArray<object[]>(data);
                        ListenForPackets();
                        handle = !handle;
                        return new Tuple<bool, object[]>(response[1] != null, response);
                    }
                }
            }

            // We failed to establish a connection
            return null;
        }

        #region Overrideables
        public virtual void OnFirstTimeConnecting(string welcomeMsg)
        {
            MessageHandler.SendMessage($"(Your ID:{ID}) {welcomeMsg}");
        }

        public abstract void OnNewMobilePartyInit(MobileParty newParty);
        public abstract void OnSetMoveGotoPoint(MobileParty party, Vec2 point);
        public abstract void OnSetMoveGotoSettlement(MobileParty party, Settlement settlement);
        #endregion

        protected override void OnPacketReceived (Packet packet)
        {
            if (packet != null)
            {
                PacketHandler.HandlePacketDel packetMethod;
                if (PacketHandler.PacketMethods.TryGetValue(packet.Command, out packetMethod))
                {
                    packetMethod.Invoke(packet);
                }
            }
        }

        private void OnTick(float dt)
        {
            OnTickEvent?.Invoke(dt);
        }

        private void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnTick);
            OnTickEvent += chat.ListenForInputThisTick;
        }
    }
}
