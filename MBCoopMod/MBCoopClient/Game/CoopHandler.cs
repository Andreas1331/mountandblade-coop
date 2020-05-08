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
        private bool isPaused;

        public CoopHandler()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnTick);
        }

        private void OnTick(float dt)
        {
            if (Campaign.Current == null)
                return;
            // Case 1: Should be paused, but isn't.
            if(isPaused && Campaign.Current.TimeControlMode != CampaignTimeControlMode.Stop)
            {
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
            }
            // Case 2: Shouldn't be paused but is not normal play speed.
            else if(!isPaused && Campaign.Current.TimeControlMode != CampaignTimeControlMode.UnstoppablePlay)
            {
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;

            }
        }

        public void ChangeGameState(bool paused)
        {
            isPaused = paused;
        }
    }
}
