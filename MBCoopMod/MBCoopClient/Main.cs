using MBCoopClient.Client_tools;
using MBCoopLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace MBCoopClient
{
    public class Main : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        protected override void OnApplicationTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.K))
            {
                Client client = new Client("127.0.0.1", 13000, Environment.UserName);
            }
        }
    }
}
