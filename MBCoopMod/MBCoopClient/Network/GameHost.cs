using MBCoopClient.Messages;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace MBCoopClient.Network
{
    public class GameHost : Client
    {
        public GameHost(string username) : base(username, true)
        {
        }

        public override void OnNewMobilePartyInit(MobileParty newParty)
        {
            //throw new NotImplementedException();
        }

        public override void OnSetMoveGotoPoint(MobileParty party, Vec2 point)
        {
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
            //throw new NotImplementedException();
        }
    }
}
