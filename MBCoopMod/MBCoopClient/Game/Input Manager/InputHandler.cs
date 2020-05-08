using MBCoopClient.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.InputSystem;

namespace MBCoopClient.Game.InputManager
{
    public class InputHandler
    {
        public void InputOnTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.K))
            {
                List<MobileParty>.Enumerator parties = MobileParty.All.GetEnumerator();
                List<MobileParty> newList = new List<MobileParty>();
                while (parties.MoveNext())
                {
                    if (parties.Current.IsMainParty)
                        continue;

                    newList.Add(parties.Current);
                }
                
                newList.ForEach(x => DestroyPartyAction.Apply(null, x));
                MessageHandler.SendMessage("Removed all! Left: " + MobileParty.All.Count);
            }

            // Open prompt for connection
            if (Input.IsKeyPressed(InputKey.F9))
            {
                ClientHandler.Instance.AttemptPromptForIpaddress();
                //handler = new CoopHandler();
            }
        }
    }
}
