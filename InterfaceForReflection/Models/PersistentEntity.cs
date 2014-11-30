using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    [Serializable]
    public class PersistentEntity<T>
    {
        public static void Save(T graph, string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                Save(graph, stream);
            }
        }

        public static void Save(T graph, Stream stream)
        {
            BinaryFormatter f = new BinaryFormatter();
            stream.Position = 0;
            f.Serialize(stream, graph);
        }

        public static T Load(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return Load(stream);
            }
        }

        public static T Load(Stream stream)
        {
            BinaryFormatter f = new BinaryFormatter();
            stream.Position = 0;

            return (T)f.Deserialize(stream);
        }

        public static T Clone(T graph)
        {
            MemoryStream stream = new MemoryStream();
            Save(graph, stream);

            return Load(stream);
        }
    }
}
