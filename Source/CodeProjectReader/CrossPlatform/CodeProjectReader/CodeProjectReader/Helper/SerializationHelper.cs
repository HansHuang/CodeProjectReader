using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CodeProjectReader.Helper
{
    /// <summary>
    /// Provides methods for serialization and De-serialization from objects in Binary Format
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Serialize object to Json(wcf)
        /// </summary>
        /// <param name="graph">object</param>
        /// <returns>json string</returns>
        public static string SerializeToWcfJson(this object graph)
        {
            using (var stream = new MemoryStream())
            {
                new DataContractJsonSerializer(graph.GetType()).WriteObject(stream, graph);
                stream.Position = 0;
                return new StreamReader(stream).ReadToEnd();
            }
        }

        /// <summary>
        /// Deserialize to object from wcf json string
        /// </summary>
        /// <param name="json">string</param>
        /// <returns>object</returns>
        public static T DeserializeFromWcfJson<T>(this string json)
        {
            using (new MemoryStream())
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }

    }

}