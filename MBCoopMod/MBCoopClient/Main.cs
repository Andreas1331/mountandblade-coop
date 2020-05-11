using HarmonyLib;
using MBCoopClient.Game;
using MBCoopClient.Game.InputManager;
using MBCoopClient.HarmonyPatches;
using MBCoopClient.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace MBCoopClient
{
    public class Main : MBSubModuleBase
    {
        private CoopHandler handler;
        private InputHandler input;

        public event Action<float> OnTickEvent;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new Harmony("CampaignSystem.TaleWorlds");
            harmony.PatchAll();
            input = new InputHandler();
            OnTickEvent += input.InputOnTick;
        }

        protected override void OnApplicationTick(float dt)
        {
            OnTickEvent.Invoke(dt);
        }
    }
}
