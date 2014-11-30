using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Storage;
using System.Transactions;
using TiS.Core.TisCommon.Transactions;

namespace TiS.Core.TisCommon.Storage.ObjectStorage
{	
	public delegate void ObjectStorageEventDelegate();

	[ComVisible(false)]
	public class ObjectStorage: ITisObjectStorage
	{
		public const string TISOBJECT_DEFAULT_EXTENSION = "TiSObject";

        private IStorageService m_storage;
		private ObjectStorageServices	  m_objectStorage;
		
		public ObjectStorage(
			IStorageService storage,
            ObjectReadDelegate objectReadDelegate, 
            ObjectWriteDelegate objectWriteDelegate)
		{
            m_storage = storage;
            m_objectStorage = new ObjectStorageServices(
                objectReadDelegate, 
                objectWriteDelegate, 
                Compression.None);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            m_storage = null;

            if (m_objectStorage != null)
            {
                m_objectStorage.Dispose();
                m_objectStorage = null;
            }
        }

        #endregion

        public object LoadObject(string name)
		{
			return m_objectStorage.LoadObject(
				m_storage,
				name
				);
		}

		public void	StoreObject(object obj, string name)
		{
            using (TransactionScope scope = TisTransactionScope.Get)
			{
				m_objectStorage.StoreObject(
					m_storage,
					name,
					obj);

                scope.Complete();
			}
		}

		public void	DeleteObject(string sName)
		{
            using (TransactionScope scope = TisTransactionScope.Get)
			{
				m_storage.DeleteStorage(new string[] {  sName });

                scope.Complete();
			}
		}

		public bool IsObjectExists(string sName)
		{
			return m_storage.IsStorageExist(sName);
		}

		public string[] QueryObjects(string nameFilter)
		{
			return m_storage.QueryStorage("", nameFilter);
		}
    }

}
