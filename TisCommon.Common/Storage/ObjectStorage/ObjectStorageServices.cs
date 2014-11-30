using System;
using System.IO;
using System.Runtime.Serialization;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Storage;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TiS.Core.TisCommon.Storage.ObjectStorage
{
    public delegate object ObjectReadDelegate(Stream stream);
    public delegate void ObjectWriteDelegate(Stream stream, object graph);

	public class ObjectStorageServices : IDisposable
	{
        private ObjectReadDelegate m_objectReadDelegate;
        private ObjectWriteDelegate m_objectWriteDelegate;
		private Compression m_compression;

        private int BUFF_SIZE = (int)FileStorageConfiguration.SmallBlobLimit;

        public ObjectStorageServices(
            ObjectReadDelegate objectReadDelegate, 
            ObjectWriteDelegate objectWriteDelegate,
            Compression compression)
		{
			m_compression   = compression;
            m_objectReadDelegate = objectReadDelegate;
            m_objectWriteDelegate = objectWriteDelegate;
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_objectReadDelegate = null;
            m_objectWriteDelegate = null;
        }

        #endregion

        public object LoadObject(byte[] data, string storageName)
        {
            using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
            {
                try
                {
                    // Uncompress if need it
                    if (UseCompression)
                    {
                        data = SafeUnzip(data, storageName);
                    }

                    // Deserialize  object
                    var dataObject = m_objectReadDelegate(new MemoryStream(data));

                    if (dataObject == null)
                    {
                        throw new TisException("Null was returned while deserialize {0}", storageName);
                    }

                    return dataObject;
                }
                catch (Exception e)
                {
                    Log.WriteError("Failed to deserialize data from {0}. Caught exception {1}", storageName, e);
                    throw;
                }
            }
        }
        /// <summary>
        /// Loads object from given storage service
        /// </summary>
        public object LoadObject(IStorageService storage, string storageName)
        {
            byte[] data = null;

            if(TryReadStorage(storage, storageName, out data) == false)
            {
                Stream readStream = storage.ReadStream(storageName);

                if (readStream == null)
                {
                    return null;
                }
                using(BinaryReader reader = new BinaryReader(readStream))
                {
                    data = new byte[reader.BaseStream.Length];
                    reader.Read(data, 0, (int)reader.BaseStream.Length);
                }    
            }

            return LoadObject(data, storageName);
        }

        public byte[] StoreObject(object obj)
        {
            using (MemoryStream stream = new MemoryStream(BUFF_SIZE))
            {
                m_objectWriteDelegate(stream, obj);

                byte[] data = stream.ToArray();

                if (UseCompression)
                {
                    data = ZipUtil.ZipArchive(data, m_compression);
                }

                return data;
            }
        }

		public void	StoreObject(IStorageService	storage, string	storageName, object	obj)
		{
            using (MemoryStream stream = new MemoryStream(BUFF_SIZE))
            {
				m_objectWriteDelegate(stream, obj);

				stream.Seek(0, SeekOrigin.Begin);

				byte[] data = StreamUtil.ReadToEnd(stream);

				if(UseCompression)
				{
					data = ZipUtil.ZipArchive(data, m_compression);
				}

				storage.WriteStorage(storageName, data);
			}
		}

		private bool UseCompression
		{
			get
			{
				return m_compression != Compression.None;
			}
		}
        /// <summary>
        /// Safe unzip. If unzip fails returns original stream
        /// </summary>
        private byte[] SafeUnzip(byte[] data, string storageName)
        {
            try
            {
                return ZipUtil.ZipUnarchive(data);
            }
            catch (Exception oExc)
            {
                Log.WriteWarning("{0}. Probably BLOB [{1}] is not compressed.", oExc.Message, storageName);

                return data;
            }
        }
        /// <summary>
        /// Tries to read storage
        /// </summary>
        private bool TryReadStorage(IStorageService storage, string storageName, out byte[] data) 
        {
            data = null;

            try
            {
                data = storage.ReadStorage(storageName);
                return data==null ? false : true;
            }
            catch (Exception e)
            {
                Log.WriteWarning("Failed to read BLOB [{0}] directly, loading from stream. Details : [{1}]", storageName, e);
                return false;
            }
        }
    
    }
}
