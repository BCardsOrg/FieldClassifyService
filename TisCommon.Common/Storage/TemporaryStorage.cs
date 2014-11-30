using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.Storage
{
    [ComVisible(false)]
    public class TemporaryStorage
    {
        private static TemporaryStorageProvider m_oStorageProvider;
        private static string m_tempPath = String.Empty;
        private static object m_locker = new object();

        public static IStorageService GetStorage()
        {
            ValidateInitialized();

            return m_oStorageProvider.GetStorage();
        }

        public static void ReleaseStorage(IStorageService oStorage)
        {
            ValidateInitialized();

            m_oStorageProvider.ReleaseStorage(oStorage);
        }

        //
        //	Private static methods
        //

        private static void ValidateInitialized()
        {
            if (m_oStorageProvider == null)
            {
                Initialize(new BasicConfiguration().eFlowTempPath);
            }
        }

        private static void Initialize(string path)
        {
            lock (m_locker)
            {
                // Check if not already initialized with the same path
                if (path != m_tempPath)
                {
                    m_oStorageProvider = new TemporaryStorageProvider(new BaseStorageImpl(path));

                    // Remember the path
                    m_tempPath = path;
                }
            }
        }
    }

    internal class TemporaryStorageProvider
    {
        private Hashtable m_oStorageToSubDirMap;
        BaseStorageImpl m_storage;
        //private string m_path;

        //
        //	Public methods
        //


        public TemporaryStorageProvider(BaseStorageImpl storage)
        {
            m_oStorageToSubDirMap = new Hashtable();

            m_storage = storage;
        }

        public IStorageService GetStorage()
        { 
            string dir = GetTempDirectory();

            m_storage.SetRelativePath(dir);

            // Keep directory name in the map
            m_oStorageToSubDirMap[m_storage] = dir;

            return m_storage;
        }

        public void ReleaseStorage(IStorageService storage)
        {
            // Get directory name from the map
            string dir = (string)m_oStorageToSubDirMap[storage];

            // Validate directory name
            if (dir != null)
            {
                // Delete directory
                storage.DeleteDir(dir);

                // Remove the storage from the map
                m_oStorageToSubDirMap.Remove(storage);
            }
        }

        private string GetTempDirectory()
        {
            return Guid.NewGuid().ToString();
        }
    }

	//internal class TemporaryStorageImpl : AppDataStorageImpl
	//{
	//    public TemporaryStorageImpl ( string path)
	//        : base(path)
	//    {
	//    }


	//}
}
