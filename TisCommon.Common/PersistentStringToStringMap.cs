using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Storage.ObjectStorage;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Storage;

namespace TiS.Core.TisCommon
{
    [ComVisible(false)]
    public class PersistentStringToStringMap
    {
        private Dictionary<string, string> m_oMap;
        private ITisObjectStorage m_oObjectStorage;
        private string m_sBLOBName;

        public PersistentStringToStringMap(
            ITransactionalStorage storage,
            string sBLOBName)
        {
            DataContractSerializer mapDataContractSerializer = new DataContractSerializer(typeof(Dictionary<string, string>));
            
            m_oObjectStorage = new ObjectStorage(
                    storage,
                    new ObjectReadDelegate(mapDataContractSerializer.ReadObject),
                    new ObjectWriteDelegate(mapDataContractSerializer.WriteObject));

            m_sBLOBName = sBLOBName;

            Load();
        }

        public Dictionary<string, string> Map
        {
            get
            {
                return m_oMap;
            }
        }

        public void Load()
        {
            try
            {
                m_oMap = (Dictionary<string, string>)m_oObjectStorage.LoadObject(
                    m_sBLOBName);
            }
            catch (Exception oExc)
            {
                Log.Write(
                    Log.Severity.INFO,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "BLOB [{0}] not loaded, {1}",
                    m_sBLOBName,
                    oExc.Message);

                m_oMap = new Dictionary<string, string>();
            }

        }

        public void Save()
        {
            m_oObjectStorage.StoreObject(
                m_oMap,
                m_sBLOBName);
        }
    }
}
