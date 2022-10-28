using MBCoopClient.Messages;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Diamond;

namespace MBCoopClient.Network
{
    public class GameClient : Client
    {
        private List<MobileParty> newParties = new List<MobileParty>();

        public GameClient(string username) : base(username, false)
        {
            OnTickEvent += OnTick;
        }

        private void OnTick(float obj)
        {
            if(newParties?.Count > 0)
            {
                try
                {
                    lock (newParties)
                    {
                        for (int i = newParties.Count - 1; i >= 0; i--)
                        {
                            MobileParty party = newParties[i];
                            if (party == null)
                                continue;

                            // Only consider deleting parties that are initialized to avoid null references
                            if (!party.Name.Equals("{=!}unnamedMobileParty"))
                            {
                                if (!party.Name.ToString().StartsWith("MBC"))
                                {
                                    DestroyPartyAction.Apply(null, party);
                                }

                                // Remove the party nomatter what, as it's either deleted or shouldn't be deleted
                                newParties.RemoveAt(i);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageHandler.SendMessage(ex.Message);
                }
            }
        }

        public override void OnFirstTimeConnecting(string welcomeMsg)
        {
            base.OnFirstTimeConnecting(welcomeMsg);
            HandleFirstTimeConnecting();
        }

        public override void OnNewMobilePartyInit(MobileParty newParty)
        {
            // Keep track of new parties and delete accordingly
            lock (newParties)
            {
                if(!newParty.IsMainParty)
                    newParties.Add(newParty);
            }
        }

        private void HandleFirstTimeConnecting()
        {
            // Client needs to send information regarding his party.
            // TODO: Delete all the clients Ai
            MobileParty party = MobileParty.MainParty;
            if (party != null)
            {
                string partyName = party.Name.ToString();
                Vector2 position = new Vector2(party.Position2D.x, party.Position2D.y);
                MobilePartyNetworkContainer container = new MobilePartyNetworkContainer(partyName, position);
                byte[] data = Packet.ObjectToByteArray(container);
                Packet packet = new Packet(Commands.SendPartyDetails, data);
                SendPacket(packet);
                MessageHandler.SendMessage("Sent party to the server..");
            }
        }

        public override void OnSetMoveGotoPoint(MobileParty party, Vec2 point)
        {
            // The client only cares about his own party
            if (party.IsMainParty)
            {
                Vector2 pos = new Vector2(point.x, point.y);
                MobilePartyNetworkContainer container = new MobilePartyNetworkContainer(party.Name.ToString(), pos);
                Packet packet = new Packet(Commands.SendPartyGotoPoint, Packet.ObjectToByteArray(container));
                SendPacket(packet);
            }
        }

        public override void OnSetMoveGotoSettlement(MobileParty party, Settlement settlement)
        {
            throw new NotImplementedException();
        }
    }
}
