using HarmonyLib;
using Helpers;
using MBCoopClient.Messages;
using MBCoopClient.Network.DataStructures;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MBCoopClient
{
    public class PropertyIgnoreSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;

        public PropertyIgnoreSerializerContractResolver()
        {
            _ignores = new Dictionary<Type, HashSet<string>>();
        }

        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
                _ignores[type] = new HashSet<string>();

            foreach (var prop in jsonPropertyNames)
                _ignores[type].Add(prop);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.PropertyType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }
            else
            {
                property.ShouldSerialize = instance =>
                {
                    try
                    {
                        PropertyInfo prop = (PropertyInfo)member;
                        if (prop.CanRead)
                        {
                            prop.GetValue(instance, null);
                            return true;
                        }
                    }
                    catch
                    {
                    }
                    return false;
                };
            }

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!_ignores.ContainsKey(type))
                return false;

            return _ignores[type].Contains(jsonPropertyName);
        }
    }

    [HarmonyPatch(typeof(MobileParty), "SetMoveGoToSettlement")]
    internal class PatchOnPartyMoveToPoint
    {
        private static void Prefix(MobileParty __instance, Settlement settlement)
        {
            if (__instance != ClientHandler.Instance.otherClient)
            {
                MessageHandler.SendMessage("SetMoveGoToSettlement!");
                MobilePartyNetworkContainer container = new MobilePartyNetworkContainer("", settlement.Position2D.x, settlement.Position2D.y);
                Packet packet = new Packet(Commands.SendPartyGotoPoint, Packet.ObjectToByteArray(container));
                if (ClientHandler.Instance.Client != null)
                {
                    ClientHandler.Instance.Client.SendPacket(packet);
                }
            }

        }
    }

    public class Main : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new Harmony("CampaignSystem.TaleWorlds");
            harmony.PatchAll();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            //ClientHandler.Instance.StartConnection();
        }

        protected override void OnApplicationTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.K))
            {
                ClientHandler.Instance.StartConnection();
            }
            else if (Input.IsKeyPressed(InputKey.H))
            {
                Packet packet = new Packet(Commands.Message, Encoding.UTF8.GetBytes("This is from the client!"));
                ClientHandler.Instance.Client.SendPacket(packet);
            }
        }
    }
}
