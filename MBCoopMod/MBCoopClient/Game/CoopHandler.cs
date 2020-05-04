using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace MBCoopClient.Game
{
    public class CoopHandler
    {
        public void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnTick);
        }

        private void OnTick(float dt)
        {
            if(Campaign.Current != null && Campaign.Current.TimeControlMode != CampaignTimeControlMode.UnstoppablePlay)
            {
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;
            }
        }
    }
}
