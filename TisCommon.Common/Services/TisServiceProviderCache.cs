using System;
using System.Collections;

namespace TiS.Core.TisCommon.Services
{
    internal class TisServiceProviderCache : IDisposable
    {
        private ICache m_oCache;
        private TisServiceProviderFactory m_oFactory;

        //
        //	Public methods
        //

        public delegate void LifetimeManagerEvent(
            string sAppName,
            ITisServiceLifetimeManager oServiceLifetimeManager);

        public event LifetimeManagerEvent OnLifetimeManagerActivate;
        public event LifetimeManagerEvent OnLifetimeManagerDeactivate;

        public TisServiceProviderCache(
            TisServiceProviderFactory oFactory)
        {
            // Keep parameters
            m_oFactory = oFactory;

            // Create cache object
            m_oCache = new MRUCache(
                true);

            // Subscribe cache events
            m_oCache.OnCacheMiss += new CacheMissEvent(OnCacheMissHandler);
            m_oCache.OnCacheRevoke += new CacheRevokeEvent(OnCacheRevokeHandler);
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_oCache.OnCacheMiss -= new CacheMissEvent(OnCacheMissHandler);
            m_oCache.OnCacheRevoke -= new CacheRevokeEvent(OnCacheRevokeHandler);
            ((MRUCache)m_oCache).Dispose();
            m_oCache = null;

            m_oFactory.Dispose();
            m_oFactory = null;
        }

        #endregion

        public ITisServiceProvider GetServiceProvider(string sAppName)
        {
            return (ITisServiceProvider)m_oCache.Get(sAppName);
        }

        public void Revoke(string sAppName)
        {
            m_oCache.Revoke(sAppName);
        }

        public void RevokeAll()
        {
            m_oCache.RevokeAll();
        }

        //
        //	Private methods
        //

        private object OnCacheMissHandler(object oSender, CacheMissEventArgs oArgs)
        {
            // Obtain application
            string sAppName = (string)oArgs.Key;

            // Use factory to return the new object
            ITisServiceProvider oServiceProvider =
                m_oFactory.CreateServiceProvider(sAppName);

            if (oServiceProvider is ITisServiceLifetimeManager)
            {
                FireOnLifetimeManagerActivate(
                    sAppName,
                    (ITisServiceLifetimeManager)oServiceProvider);
            }

            return oServiceProvider;
        }

        private void OnCacheRevokeHandler(object oSender, CacheRevokeEventArgs oArgs)
        {
            if (oArgs.EventTiming != CacheRevokeEventArgs.Timing.Post)
            {
                return;
            }

            string sAppName = (string)oArgs.Key;

            ITisServiceProvider oServiceProvider =
                (ITisServiceProvider)oArgs.Value;

            if (oServiceProvider is ITisServiceLifetimeManager)
            {
                FireOnLifetimeManagerDeactivate(
                    sAppName,
                    (ITisServiceLifetimeManager)oServiceProvider);
            }
        }

        private void FireOnLifetimeManagerActivate(
            string sAppName,
            ITisServiceLifetimeManager oLifetimeManager)
        {
            if (OnLifetimeManagerActivate != null)
            {
                OnLifetimeManagerActivate(sAppName, oLifetimeManager);
            }
        }

        private void FireOnLifetimeManagerDeactivate(
            string sAppName,
            ITisServiceLifetimeManager oLifetimeManager)
        {
            if (OnLifetimeManagerDeactivate != null)
            {
                OnLifetimeManagerDeactivate(sAppName, oLifetimeManager);
            }
        }
    }
}
