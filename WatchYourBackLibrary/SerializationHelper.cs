using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace WatchYourBackLibrary
{
    /// <summary>
    /// Contains methods to serialize and deserialize information
    /// </summary>
    public static class SerializationHelper
    {
        public static byte[] Serialize(object objectToSerialize)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, objectToSerialize);
            byte[] result = new Byte[stream.Length];
            stream.Position = 0;
            stream.Read(result, 0, (int)stream.Length);
            stream.Close();
            return result;

        }

        public static T DeserializeObject<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data);
            object result = formatter.Deserialize(stream);
            stream.Close();
            return (T)result;
        }
    }
}
