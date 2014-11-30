using System;
using System.Runtime.Serialization;
using System.IO;

namespace TiS.Core.TisCommon.Storage
{
    [Serializable]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class StorageInfo : IStorageInfo
    {
        [DataMember(Name = "Name")]
        private string m_sName;
        [DataMember(Name = "SizeBytes")]
        private long m_lSizeBytes;
        [DataMember(Name = "LastModified")]
        private DateTime m_oLastModified;
        [DataMember(Name = "CRC")]
        private int m_nCRC32;
        [DataMember(Name = "DataFlags")]
        StorageInfoFlags m_enDataFlags;

        public StorageInfo()
        {
        }

        public StorageInfo(FileInfo fileInfo)
        {
            m_sName =  Path.GetFileName(fileInfo.Name);
            m_lSizeBytes = fileInfo.Length;
            m_oLastModified = fileInfo.LastWriteTime;
            m_enDataFlags = StorageInfoFlags.All;
        }

        public StorageInfo(
            string sName,
            long lSizeBytes,
            int nCRC32,
            DateTime oLastModified,
            StorageInfoFlags enDataFlags)
        {
            m_sName = sName;
            m_lSizeBytes = lSizeBytes;
            m_nCRC32 = nCRC32;
            m_oLastModified = oLastModified;
            m_enDataFlags = enDataFlags;
        }


        #region IStorageInfo Members

        public string Name
        {
            get 
            { 
                return m_sName; 
            }
            set
            {
                m_sName = value;
            }
        }

        public long SizeBytes
        {
            get 
            { 
                return m_lSizeBytes; 
            }
            set
            {
                m_lSizeBytes = value;
            }
        }

        public DateTime LastModified
        {
            get
            {
                return m_oLastModified;
            }
        }

        public int CRC32
        {
            get { return m_nCRC32; }
        }

        public StorageInfoFlags DataFlags
        {
            get { return m_enDataFlags; }
        }

        public bool SizeValid
        {
            get { return FlagSet(m_enDataFlags, StorageInfoFlags.Size); }
        }

        public bool LastModifiedValid
        {
            get { return FlagSet(m_enDataFlags, StorageInfoFlags.LastModified); }
        }

        public bool CRC32Valid
        {
            get { return FlagSet(m_enDataFlags, StorageInfoFlags.CRC32); }
        }

        #endregion

        public static bool FlagSet(
            StorageInfoFlags enFlags,
            StorageInfoFlags enFlagMask)
        {
            return (enFlags & enFlagMask) > 0;
        }
    }
}
