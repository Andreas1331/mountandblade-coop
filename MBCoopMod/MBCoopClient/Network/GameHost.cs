using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace MBCoopClient.Network
{
    public class GameHost : Client
    {
        public GameHost(string username) : base(username, true)
        {
        }

        public override void OnNewMobilePartyInit(MobileParty newParty)
        {
            throw new NotImplementedException();
        }
    }
}
