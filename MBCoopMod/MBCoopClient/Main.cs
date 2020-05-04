using HarmonyLib;
using MBCoopClient.Game;
using MBCoopClient.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
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
                //CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, (mb, pb) =>
                //{
                //    MessageHandler.SendMessage("Destroyed: " + mb.Name.ToString());
                //});

                //List<MobileParty>.Enumerator lstParties = MobileParty.All.GetEnumerator();
                //List<MobileParty> newList = new List<MobileParty>();
                //while (lstParties.MoveNext())
                //{
                //    if (lstParties.Current != null)
                //    {
                //        if (!lstParties.Current.IsMainParty)
                //            newList.Add(lstParties.Current);
                //    }
                //}
                //newList.ForEach(x =>
                //{
                //    DestroyPartyAction.Apply(null, x);
                //});
                //MobileParty k = new MobileParty();
                //new MobileParty();
                //new MobileParty();
                //new MobileParty();
                //new MobileParty();
                //new MobileParty();
                //new MobileParty();
                //new MobileParty();
                //MessageHandler.SendMessage("Created!");
                //return;

                //MBObjectManager.Instance.CreateObject<MobileParty>("fas2").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5); ;
                //MBObjectManager.Instance.CreateObject<MobileParty>("fas").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //MBObjectManager.Instance.CreateObject<MobileParty>("fas23").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //MBObjectManager.Instance.CreateObject<MobileParty>("fas5").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //MBObjectManager.Instance.CreateObject<MobileParty>("fas6").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //MBObjectManager.Instance.CreateObject<MobileParty>("fas78").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //MBObjectManager.Instance.CreateObject<MobileParty>("fas8").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //MBObjectManager.Instance.CreateObject<MobileParty>("fas123").InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //MBObjectManager.Instance.CreateObject<MobileParty>();
                //MBObjectManager.Instance.CreateObject<MobileParty>();
                //MBObjectManager.Instance.CreateObject<MobileParty>();
                //otherClient.InitializeMobileParty(new TextObject("Players party"), new TroopRoster(), new TroopRoster(), MobileParty.MainParty.Position2D, 5);
                //otherClient.Party.Visuals.SetMapIconAsDirty();
                //otherClient.IsLordParty = true;
                ////otherClient.DisableAi();
                //otherClient.Ai.SetDoNotMakeNewDecisions(true);
                //otherClient.Party.AddMembers(MobileParty.MainParty.MemberRoster.ToFlattenedRoster());
                //MessageHandler.SendMessage("Length: " + MobileParty.All.Count);

                ClientHandler.Instance.StartConnection();
                coop = new CoopHandler();
                coop.RegisterEvents();
            }
        }
        CoopHandler coop;
    }
}
