using MBCoopClient.HarmonyPatches;
using MBCoopClient.Messages;
using MBCoopClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace MBCoopClient.Game.InputManager
{
    public class InputHandler
    {
        public void InputOnTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.B))
            {
                List<MobileParty>.Enumerator parties = MobileParty.All.GetEnumerator();

                List<MobileParty> newList = new List<MobileParty>();
                while (parties.MoveNext())
                {
                    if (parties.Current.IsMainParty)
                        continue;

                    newList.Add(parties.Current);
                }
                int count = newList.Count;
                newList.ForEach(x => DestroyPartyAction.Apply(null, x));
                ClientHandler.Instance.Client = new GameClient("Andreas");
                MessageHandler.SendMessage("Client created..");
            }

            if (Input.IsKeyPressed(InputKey.K))
            {
                //MobileParty party = new MobileParty();
                //party.StringId = "HIHIHIHI";
                //MBObjectManager.Instance.RegisterObject<MobileParty>(party);

                //MBObjectManager.Instance.CreateObject<MobileParty>("fas2").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5); ;

                List<MobileParty>.Enumerator parties = MobileParty.All.GetEnumerator();

                List < MobileParty> newList = new List<MobileParty>();
                while (parties.MoveNext())
                {
                    if (parties.Current.IsMainParty)
                        continue;

                    newList.Add(parties.Current);
                }
                int count = newList.Count;
                newList.ForEach(x => DestroyPartyAction.Apply(null, x));
                
                MessageHandler.SendMessage("Removed " + count + "! Left: " + MobileParty.All.Count);
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
