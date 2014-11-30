using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Serialization
{
	/// <summary>
	/// A DataContractSerializer that use Binary format
	/// Also remove common xml namespace
	/// </summary>
	public class DataContractSerializerBinary : XmlObjectSerializer
    {
        #region Private fields
        private static readonly XmlDictionaryReaderQuotas m_Quotas = new XmlDictionaryReaderQuotas()
        {
            MaxDepth = 500,
            MaxBytesPerRead = Int32.MaxValue,
            MaxStringContentLength = Int32.MaxValue
        };
        
        private readonly DataContractSerializer serializer;

		private readonly List<string> namespaces = new List<string>();
        
        #endregion

        #region Ctor
        public DataContractSerializerBinary(Type type)
		{
			serializer = new DataContractSerializer(type);
			InitNamespaces(type, null);
		}

		public DataContractSerializerBinary(Type type, IEnumerable<Type> knownTypes)
		{
			serializer = new DataContractSerializer(type, knownTypes);
			InitNamespaces(type, knownTypes);
		}

		public DataContractSerializerBinary(Type type, string rootName, string rootNamespace)
		{
			serializer = new DataContractSerializer(type, rootName, rootNamespace);
			InitNamespaces(type, null);
		}

		public DataContractSerializerBinary(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
		{
			serializer = new DataContractSerializer(type, rootName, rootNamespace);
			InitNamespaces(type, null);
		}

		public DataContractSerializerBinary(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes)
		{
			serializer = new DataContractSerializer(type, rootName, rootNamespace, knownTypes);
			InitNamespaces(type, knownTypes);
		}

		public DataContractSerializerBinary(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes)
		{
			serializer = new DataContractSerializer(type, rootName, rootNamespace, knownTypes);
			InitNamespaces(type, knownTypes);
		}

		public DataContractSerializerBinary(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, 
            bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
		{
			serializer = new DataContractSerializer(type, knownTypes, maxItemsInObjectGraph, 
                ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
			InitNamespaces(type, knownTypes);
		}

		public DataContractSerializerBinary(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
		{
			serializer = new DataContractSerializer(type, rootName, rootNamespace, knownTypes, 
                maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
			InitNamespaces(type, knownTypes);
		}

		public DataContractSerializerBinary(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, 
            int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
		{
			serializer = new DataContractSerializer(type, rootName, rootNamespace, knownTypes, 
                maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
			InitNamespaces(type, knownTypes);
		}
        #endregion

        private void InitNamespaces(Type mainType, IEnumerable<Type> knownTypes)
		{
			// Dummy item
			namespaces.Add(@"http://schemas.datacontract.org/2004/07/System.Drawing");

			namespaces.Add(@"http://schemas.datacontract.org/2004/07/System.Drawing");
			namespaces.Add(@"http://schemas.microsoft.com/2003/10/Serialization/Arrays");
			namespaces.Add("http://schemas.datacontract.org/2004/07/TiS.Core.Application.Workflow");
			namespaces.Add(@"http://www.topimagesystems.com/eFlow");
		}

		public override void WriteObject(Stream stream, object graph)
		{
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream);
            
            WriteObject(writer, graph);
            writer.Flush();
		}

		public override object ReadObject(Stream stream)
		{
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(stream, m_Quotas);
            return ReadObject(reader);
		}

		public override void WriteObject(XmlDictionaryWriter writer, object graph)
		{
			WriteStartObject(writer, graph);

			// Skip first default namespace (standard xmlns without alias)
			// and add the following namespace with x[0-9]+ alias
			for (int i = 1; i < namespaces.Count; i++)
			{
				string aliasNamespace = "x" + ((i > 1) ? (i - 1).ToString() : "");

				writer.WriteAttributeString("xmlns", aliasNamespace, null, namespaces[i]);
			}

			WriteObjectContent(writer, graph);
			WriteEndObject(writer);
		}

		public override void WriteObject(XmlWriter writer, object graph)
		{
			WriteObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
		{
			serializer.WriteStartObject(writer, graph);
		}

		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			serializer.WriteObjectContent(writer, graph);
		}

		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			serializer.WriteEndObject(writer);
		}

		public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
		{
			return serializer.ReadObject(reader, verifyObjectName);
		}

		public override bool IsStartObject(XmlDictionaryReader reader)
		{
			return serializer.IsStartObject(reader);
		}
	}
}
