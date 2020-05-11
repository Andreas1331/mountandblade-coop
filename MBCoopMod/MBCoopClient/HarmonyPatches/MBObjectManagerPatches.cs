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
using TaleWorlds.ObjectSystem;

namespace MBCoopClient.HarmonyPatches
{
	[HarmonyPatch]
	public static class MBObjectManagerPatches
	{
		public static MethodBase TargetMethod()
		{
			return AccessTools.Method(typeof(MBObjectManager), "RegisterObject").MakeGenericMethod(typeof(MobileParty));
		}

		static void Postfix(object obj)
		{
			MessageHandler.SendMessage("Party: " + obj.GetType());
		}
	}
}
