using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace TiS.Core.TisCommon.Serialization
{
    public static class DataContractSerialization
    {
        public static T DeserializeObject<T>(string str, IEnumerable<Type> knownTypes = null, IDataContractSurrogate surrogate = null)
        {
            T result = default(T);

            DataContractSerializer serializer = new DataContractSerializer(typeof(T), knownTypes, Int32.MaxValue, false, true, surrogate);
            using (MemoryStream stream = new MemoryStream(UnicodeEncoding.Default.GetBytes(str)))
            {
                result = (T)serializer.ReadObject(stream);
            }
            return result;
        }

        public static string SerializeObject(object obj, IEnumerable<Type> knownTypes = null, IDataContractSurrogate surrogate = null)
        {
            DataContractSerializer serializer = new DataContractSerializer(obj.GetType(), knownTypes, Int32.MaxValue, false, true, surrogate);

            string str = null;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                str = UnicodeEncoding.Default.GetString(stream.ToArray());
            }

            return str;
        }
    }
}
