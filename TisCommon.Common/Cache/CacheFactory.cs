using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon.Analytics;
using System.Diagnostics;
using System.ServiceModel;

namespace TiS.Core.TisCommon.Cache
{
    public static class CacheFactory
    {
        #region Private static fields

        private static IDataCache m_Cache = null;

		private static DictionaryDataCache m_LocalCache = new DictionaryDataCache();

        private static object m_Locker = new object();
        
        #endregion

        #region Public methods
        /// <summary>
        /// Port number
        /// </summary>
        public static int CachePort { get; set; }
        /// <summary>
        /// Host name
        /// </summary>
        public static string CacheHost { get; set; }
        /// <summary>
        /// Applications Name
        /// </summary>
        public static string CacheApplication { get; set; }
        /// <summary>
        /// Cache for local usage only. TODO: check .NET MemoryCache 
        /// </summary>
        public static IDataCache LocalCache
        {
            get { return m_LocalCache; }
        }
        /// <summary>
        /// Returns cache
        /// </summary>
        public static IDataCache Get()
        {
            if (m_Cache == null)
            {
                Initialize();
            }
            
            return m_Cache;
        }

        public static void InitializeAppFabricSettings(string host, int port, string application)
        {
            CacheHost = host;
            CachePort = port;
            CacheApplication = application;
            
            Initialize();
        }
        #endregion

        #region Private methods
        private static void Initialize()
        {
            lock (m_Locker)
            {
                if (m_Cache == null)
                {
                    if (CanCreateAppFabricCache(CacheHost, CachePort, CacheApplication, out m_Cache) == false)
                    {
                        m_Cache = new DictionaryDataCache();
                    }
                }
            }
        }
        /// <summary>
        /// Creates appfabric cache and tests its basic functionality
        /// </summary>
        private static bool CanCreateAppFabricCache(string host, int port, string application, out IDataCache appFabricCache)
        {
            try
            {
                var appFabricCacheType = Type.GetType("TiS.Core.TisCommon.Cache.AppFabricDataCache, TiS.Core.TisCommon.Server", true);
                
                var cache = (IDataCache)Activator.CreateInstance(appFabricCacheType);
                cache.ActivateCache(host, port, application);
                
                // Check that hash is healthy
                cache.GetCacheItem("dummy");

                appFabricCache = cache;
                return true;
            }
            catch (Exception e)
            {
                Log.WriteWarning("Failed to create appFabricCache. Exception: {0}", e);

                appFabricCache = null;
                return false;
            }
        }
        #endregion
    }
}
