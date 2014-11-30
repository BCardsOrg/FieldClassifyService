using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Storage;

namespace TiS.Core.TisCommon.Client.Storage
{	
	public delegate void ObjectStorageEventDelegate();

	[ComVisible(false)]
	public class TisObjectStorage: ITisObjectStorage
	{
		public const string TISOBJECT_DEFAULT_EXTENSION = "TiSObject";
	
		private ITransactionalStorageService m_storage;
		private ObjectStorageServices	  m_objectStorage;
		
		private ObjectStorageEventDelegate OnPreLoad;
		private ObjectStorageEventDelegate OnPostLoad;

		//
		//	Public methods
		//

		public TisObjectStorage(
			ITransactionalStorageService	storage,
			IFormatter					            formatter)
		{
            m_storage = storage;

            m_objectStorage = new ObjectStorageServices(
                formatter,
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
				AddExtensionIfNeeded(name));
		}

		public void	StoreObject(object obj, string name)
		{
			// Begin transaction if needed
			using(AutoTransaction oTx = new AutoTransaction(m_storage))
			{
				// Store object
				m_objectStorage.StoreObject(
					m_storage,
					AddExtensionIfNeeded(name),
					obj);
				
				// Success
				oTx.SetCommit();
			}
		}

		public void	DeleteObject(string sName)
		{
			using(AutoTransaction oTx = new AutoTransaction(m_storage))
			{
				m_storage.DeleteStorage(
					"", 
					new string[] { AddExtensionIfNeeded( sName ) }
					);
				
				// Success
				oTx.SetCommit();
			}
		}

		public bool IsObjectExists(string sName)
		{
			return m_storage.IsStorageExist(
				"", 
				AddExtensionIfNeeded( sName )
				);
		}

		public string[] QueryObjects(string nameFilter)
		{
			return m_storage.QueryStorage("", nameFilter);
		}

		//
		//	Private 
		//

		private string AddExtensionIfNeeded(string storageName)
		{
			if(Path.GetExtension(storageName).Length == 0)
			{
				return String.Format(
					"{0}.{1}", storageName, TISOBJECT_DEFAULT_EXTENSION);
			}

			return storageName;
		}
    }

}
