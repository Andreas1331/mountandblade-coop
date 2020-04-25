using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace MBCoopClient
{
    public class Main : MBSubModuleBase
    {
        protected override void OnApplicationTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.K))
            {
                InformationManager.DisplayMessage(new InformationMessage("Hey from Coop!"));
            }
        }
    }
}
