using Helpers;
using MBCoopClient.Messages;
using MBCoopLibrary;
using MBCoopLibrary.NetworkData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MBCoopClient
{
    /// <summary>
    /// Special JsonConvert resolver that allows you to ignore properties.  See https://stackoverflow.com/a/13588192/1037948
    /// </summary>
    public class IgnorableSerializerContractResolver : DefaultContractResolver
    {
        protected readonly Dictionary<Type, HashSet<string>> Ignores;

        public IgnorableSerializerContractResolver()
        {
            Ignores = new Dictionary<Type, HashSet<string>>();
        }

        /// <summary>
        /// Explicitly ignore the given property(s) for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName">one or more properties to ignore.  Leave empty to ignore the type entirely.</param>
        public void Ignore(Type type, params string[] propertyName)
        {
            // start bucket if DNE
            if (!Ignores.ContainsKey(type)) Ignores[type] = new HashSet<string>();

            foreach (var prop in propertyName)
            {
                Ignores[type].Add(prop);
            }
        }

        /// <summary>
        /// Is the given property for the given type ignored?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsIgnored(Type type, string propertyName)
        {
            if (!Ignores.ContainsKey(type)) return false;

            // if no properties provided, ignore the type entirely
            if (Ignores[type].Count == 0) return true;

            return Ignores[type].Contains(propertyName);
        }

        /// <summary>
        /// The decision logic goes here
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // Check if this is a property to ignore
            if (this.IsIgnored(property.DeclaringType, property.PropertyName))
            //|| this.IsIgnored(property.DeclaringType.BaseType, property.PropertyName))
            {
                property.ShouldSerialize = instance => 
                { 
                    return false; 
                };
            }

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

            return property;
        }
    }

    public class PropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;

        public PropertyRenameAndIgnoreSerializerContractResolver()
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
                ClientHandler.Instance.StartConnection();
            }
            else if (Input.IsKeyPressed(InputKey.H))
            {
                MobileParty party = MobileParty.MainParty;
                MessageHandler.SendMessage("Party: " + party.Name.ToString());

                var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
                jsonResolver.IgnoreProperty(typeof(Settlement), "LastVisitedSettlement");
                jsonResolver.IgnoreProperty(typeof(Settlement), "TargetSettlement");
                jsonResolver.IgnoreProperty(typeof(Settlement), "HomeSettlement");
                jsonResolver.IgnoreProperty(typeof(Settlement), "CurrentSettlement");
                jsonResolver.IgnoreProperty(typeof(Settlement), "BesiegedSettlement");
                jsonResolver.IgnoreProperty(typeof(Settlement), "ShortTermTargetSettlement");
                jsonResolver.IgnoreProperty(typeof(PartyBase), "Party");
                jsonResolver.IgnoreProperty(typeof(PartyBase), "AiBehaviorObject");
                jsonResolver.IgnoreProperty(typeof(Hero), "EffectiveScout");
                jsonResolver.IgnoreProperty(typeof(Hero), "EffectiveQuartermaster");
                jsonResolver.IgnoreProperty(typeof(Hero), "EffectiveEngineer");
                jsonResolver.IgnoreProperty(typeof(Hero), "EffectiveSurgeon");
                jsonResolver.IgnoreProperty(typeof(Hero), "Quartermaster");
                jsonResolver.IgnoreProperty(typeof(Hero), "Scout");
                jsonResolver.IgnoreProperty(typeof(Hero), "LeaderHero");
                jsonResolver.IgnoreProperty(typeof(Hero), "Engineer");
                jsonResolver.IgnoreProperty(typeof(Hero), "Surgeon");
                jsonResolver.IgnoreProperty(typeof(Hero), "EscortHero");
                jsonResolver.IgnoreProperty(typeof(CharacterObject), "Leader");
                jsonResolver.IgnoreProperty(typeof(IFaction), "MapFaction");

                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = jsonResolver;

                var data = JsonConvert.SerializeObject(party, serializerSettings);

                MobileParty newParty = JsonConvert.DeserializeObject<MobileParty>(data);
                MessageHandler.SendMessage("NewParty: " + newParty.Name.ToString());

                //var jsonResolver = new IgnorableSerializerContractResolver();
                //// ignore single property
                //jsonResolver.Ignore(typeof(Settlement), "LastVisitedSettlement");
                //// ignore single datatype
                //var jsonSettings = new JsonSerializerSettings() { ContractResolver = jsonResolver };
                //string data = JsonConvert.SerializeObject(party, Formatting.Indented, jsonSettings);

                Packet packet = new Packet(Commands.Message, Encoding.UTF8.GetBytes("This is from the client!"));
                ClientHandler.Instance.Client.SendPacket(packet);
            }
        }

        public static byte[] ObjectToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
