using HarmonyLib;
using MBCoopClient.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.Core.MBObjectManager;

namespace MBCoopClient.HarmonyPatches
{
    [HarmonyPatch(typeof(ObjectTypeRecord<MobileParty>))]
    public class ObjectTypeRecordPatches
    {
        [HarmonyTargetMethod]
        private static MethodBase TargetMethod() => typeof(ObjectTypeRecord<MobileParty>).GetMethod("RegisterObject", new Type[] { typeof(MobileParty), typeof(bool), typeof(MBObjectBase) });

        [HarmonyPostfix]
        public static void Postfix(MobileParty obj, bool presumed, MBObjectBase registeredObject)
        {
            if(obj is MobileParty mb)
            {
                MessageHandler.SendMessage("Created new party! : " + mb.GetType() + " - Name: " + registeredObject.GetName());
            }
        }
    }
}
