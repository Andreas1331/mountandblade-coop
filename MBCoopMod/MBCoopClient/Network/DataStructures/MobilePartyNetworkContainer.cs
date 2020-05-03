using System;

namespace MBCoopClient.Network.DataStructures
{
    [Serializable]
    public class MobilePartyNetworkContainer
    {
        public string Name;
        public Vector2 Position;

        public MobilePartyNetworkContainer(string name, Vector2 position)
        {
            Name = name;
            Position = position;
        }
    }
}
