using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Serialization
{
	/// <summary>
	/// A DataContractSerializer that use Binary format + Zip the data
	/// </summary>
	public class DataContractSerializerZip : DataContractSerializerBinary
	{
		#region DataContractSerializerBinary

		public DataContractSerializerZip(Type type)
			: base(type)
		{ }

		public DataContractSerializerZip(Type type, IEnumerable<Type> knownTypes)
			: base(type, knownTypes)
		{ }


		public DataContractSerializerZip(Type type, string rootName, string rootNamespace)
			: base(type, rootName, rootNamespace)
		{ }


		public DataContractSerializerZip(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
			: base(type, rootName, rootNamespace)
		{ }


		public DataContractSerializerZip(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes)
			: base(type, rootName, rootNamespace, knownTypes)
		{ }


		public DataContractSerializerZip(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes)
			: base(type, rootName, rootNamespace, knownTypes)
		{ }


		public DataContractSerializerZip(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: base(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate)
		{ }


		public DataContractSerializerZip(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: base(type, rootName, rootNamespace, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate)
		{ }


		public DataContractSerializerZip(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: base(type, rootName, rootNamespace, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate)
		{ }

		#endregion

		public override void WriteObject(Stream stream, object graph)
		{
			using (DeflateStream xx = new DeflateStream(stream, CompressionMode.Compress))
			{
				base.WriteObject(xx, graph);
			}
		}

		public override object ReadObject(Stream stream)
		{
			using (DeflateStream xx = new DeflateStream(stream, CompressionMode.Decompress))
			{
				return base.ReadObject(xx);
			}
		}

		public void WriteObjectItem(Stream stream, Func<int, object> getItem)
		{
			DateTime zipTime = DateTime.Now;
			using (ZipStorer zipStorer = ZipStorer.Create(stream))
			{
				object item;
				int i = 0;
				while ((item = getItem(i)) != null)
				{
					using (MemoryStream unzipStraem = new MemoryStream())
					{
						base.WriteObject(unzipStraem, item);

						unzipStraem.Position = 0;
						zipStorer.AddStream(ZipStorer.Compression.Deflate, i.ToString(), unzipStraem, zipTime);
					}
					i++;
				}
			}
		}

		public void ReadObjectItem(Stream stream, Func<object, int, bool> putItem)
		{
			stream.Position = 0;
			using (ZipStorer zipStorer = ZipStorer.Open(stream, FileAccess.Read))
			{
				int noOfItems = zipStorer.ReadCentralDir().Count;
				for (int i = 0; i < noOfItems; i++)
				{
					//zipStorer.ExtractFile(new ZipStorer.ZipFileEntry() { FilenameInZip = index.ToString() }, unzipStraem);
					using (var unzipStraem = new MemoryStream())
					{
						zipStorer.ExtractFile(zipStorer.ReadCentralDir()[i], unzipStraem);
						unzipStraem.Position = 0;
						putItem(base.ReadObject(unzipStraem), i);
					}
				}
			}
		}
	}
}
