using HarmonyLib;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using System.Text;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace MBCoopClient
{
    public class Main : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new Harmony("CampaignSystem.TaleWorlds");
            harmony.PatchAll();
        }

        //public override void OnGameInitializationFinished(Game game)
        //{
        //    base.OnGameInitializationFinished(game);
        //    //ClientHandler.Instance.StartConnection();
        //}

        protected override void OnApplicationTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.K))
            {
                ClientHandler.Instance.StartConnection();
            }
            else if (Input.IsKeyPressed(InputKey.H))
            {
                Packet packet = new Packet(Commands.SendMessage, Encoding.UTF8.GetBytes("This is from the client!"));
                ClientHandler.Instance.Client.SendPacket(packet);
            }
        }
    }
}
