﻿using MBCoopClient.Messages;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System;
using System.Collections.Generic;
using System.Timers;
using TaleWorlds.CampaignSystem.Actions;
using System.Linq;
using System.Reflection;

namespace MBCoopClient.Network
{
    public class GameClient : Client
    {
        private List<MobileParty> newParties = new List<MobileParty>();
        private Timer deletePartiesTimer;

        public GameClient(string username) : base(username, false)
        {
            deletePartiesTimer = new Timer(2000);
            deletePartiesTimer.Elapsed += OnTimedEvent;
            deletePartiesTimer.AutoReset = true;
            deletePartiesTimer.Enabled = true;
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
                newParties.Add(newParty);
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            return;
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
                                MessageHandler.SendMessage($"Deleted party: " + party.Name);
                                DestroyPartyAction.Apply(null, party);
                                newParties.RemoveAt(i);
                            }
                        }
                    }

                    //foreach (MobileParty party in newParties)
                    //{
                    //    if (party == null)
                    //        continue;
                    //    // Only consider deleting parties that are initialized to avoid null references
                    //    if (!party.Name.Equals("{=!}unnamedMobileParty"))
                    //    {
                    //        if (!party.Name.ToString().StartsWith("MBC"))
                    //        {
                    //            MessageHandler.SendMessage($"Deleted party: " + party.Name);
                    //            if(party != null)
                    //                DestroyPartyAction.Apply(null, party);
                    //            newParties[newParties.IndexOf(party)] = null;
                    //        }
                    //    }
                    //}
                }
            }
            catch(Exception ex)
            {
                MessageHandler.SendMessage(ex.Message);
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
                Vec2 position = party.GetPosition2D;
                MobilePartyNetworkContainer container = new MobilePartyNetworkContainer(partyName, position.x, position.y);
                byte[] data = Packet.ObjectToByteArray(container);
                Packet packet = new Packet(Commands.SendPartyDetails, data);
                SendPacket(packet);
                MessageHandler.SendMessage("Sent party to the server..");
            }
        }
    }
}