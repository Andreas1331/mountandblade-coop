using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace MBCoopLibrary.NetworkData
{
    public class Packet
    {
        [JsonProperty]
        public Commands Command { get; set; }
        [JsonProperty]
        public byte[] Data { get; set; }

        public Packet(Commands cmd, byte[] data)
        {
            Command = cmd;
            Data = data;
        }

        // Serialize to Json
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Deserialize
        public static Packet FromJson(string jsonData)
        {
            return JsonConvert.DeserializeObject<Packet>(jsonData);
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
