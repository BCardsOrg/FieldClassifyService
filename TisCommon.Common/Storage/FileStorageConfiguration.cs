using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Configuration;
using System.Runtime.Serialization;


namespace TiS.Core.TisCommon.Storage
{
	public class FileStorageConfiguration 
	{
        private const string SECTION_NAME = "StorageConfig";

        private static FileStorageConfigurationData m_configObject ;

        static FileStorageConfiguration()
        {
            m_configObject = Load();
        }

        private static FileStorageConfigurationData Load()
        {
            if (m_configObject == null)
            {
                GlobalConfigurationService storage = new GlobalConfigurationService();

                m_configObject = (FileStorageConfigurationData)storage.Load(SECTION_NAME);
            }

            return m_configObject;
        }

        public static long SmallBlobLimit
        {
            get 
            {
                if (m_configObject == null)
                {
                    return FileStorageConfigurationData.SMALL_BLOB_LIMIT ;
                }
                else
                {
                    return m_configObject.SmallBlobLimit;
                }
            }
        }
    }

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class FileStorageConfigurationData
    {            
        public const long SMALL_BLOB_LIMIT = 1024 * 1024 * 3;
        private long m_smallBlobLimit = SMALL_BLOB_LIMIT;

        [DataMember]
		public long SmallBlobLimit
		{
			get
			{      
				return m_smallBlobLimit;
			}
            set 
            {
               m_smallBlobLimit = value;
            }
		}
    }
}
