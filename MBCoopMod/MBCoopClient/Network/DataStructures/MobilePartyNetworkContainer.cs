using System;

namespace MBCoopClient.Network.DataStructures
{
    [Serializable]
    public class MobilePartyNetworkContainer
    {
        public string Name;
        public float PosX;
        public float PosY;

        public MobilePartyNetworkContainer(string name, float posX, float posY)
        {
            Name = name;
            PosX = posX;
            PosY = posY;
        }
    }
}
