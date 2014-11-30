using System;
using System.Collections;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Customizations;
using System.Threading;

namespace TiS.Core.TisCommon.Services
{
    [ComVisible(false)]
	internal class TisServiceCache: 
		ITisServiceProvider,
		ITisServiceLifetimeManager,
		IDisposable
	{
		private ICache				m_oServicesCache;
		private ITisServiceProvider m_oServicesActivator;

		// Element type: << ServiceKey >>
        private ConcurrentList<TisServiceKey> m_oServicesLoadOrder = new ConcurrentList<TisServiceKey>();
		
		private string m_sAppName;

		private bool m_bDisposing = false;

		//
		//	Public
		//

		public event TisServiceLifetimeEvent   OnPreServiceActivate;
        public event TisServiceLifetimeEventEx OnPostServiceActivate;
        public event TisServiceLifetimeEventEx OnPreServiceDeactivate;
        public event TisServiceLifetimeEvent OnPostServiceDeactivate;

		public TisServiceCache(
			ITisServiceProvider oServicesActivator,
			string			    sAppName)
		{
			m_sAppName = sAppName;

			// Keep the service provider
			m_oServicesActivator = oServicesActivator;

			// Create services cache
			m_oServicesCache = new MRUCache(
				false // DisposeOnRevoke
				);
			
			// Subscribe to the cache "OnCacheMiss" event
			m_oServicesCache.OnCacheMiss += 
				new CacheMissEvent(ServicesCache_OnCacheMiss);
		}

		public string AppName
		{
			get { return m_sAppName; }
		}

		public object GetService(
			TisServiceKey oServiceKey)
		{
			return m_oServicesCache.Get(oServiceKey);
		}

        public bool IsInstantiated(TisServiceKey oServiceKey)
        {
            return m_oServicesCache.IsInCache(oServiceKey);
        }

        public void AddService(
			TisServiceKey oServiceKey,
			object     oService)
		{
			m_oServicesCache.Put(oServiceKey, oService);	
		}

        public void ReleaseService(
            TisServiceKey oServiceKey,
            ITisServiceInfo serviceInfo = null,
            object oService = null)
        {
            if (oService == null && IsInstantiated(oServiceKey))
            {
                oService = GetService(oServiceKey);
            }

            if (oService != null)
            {
                if (serviceInfo != null)
                {
                    TisServiceEventsAdapterBuilder.ReleaseService(serviceInfo, oService);
                }

                PerformReleaseService(oServiceKey);

                PerformDisposeService(oService);
            }

            // Fire post-deactivate event
            FirePostServiceDeactivate(oServiceKey);

            int nIndex;

            nIndex = m_oServicesLoadOrder.IndexOf(oServiceKey);

            if (nIndex >= 0)
            {
                m_oServicesLoadOrder.RemoveAt(nIndex);
            }
        }

		#region IDisposable Members

        public void Dispose()
        {
            m_bDisposing = true;

            if (m_oServicesLoadOrder.Count == 0)
            {
                return;
            }

            TisServiceKey[] servicesLoadOrderArray = new TisServiceKey[m_oServicesLoadOrder.Count];
            m_oServicesLoadOrder.CopyTo(servicesLoadOrderArray, 0);

            // Allow calling Dispose on the services.
            // Scan in the reverse direction of load order
            for (int i = servicesLoadOrderArray.Length - 1; i > 0; i--)
            {
                TisServiceKey serviceKey = servicesLoadOrderArray[i];
                //Log.Write(
                //    Log.Severity.DETAILED_DEBUG,
                //    System.Reflection.MethodInfo.GetCurrentMethod(),
                //    "Deactivating service [{0}] in App [{1}]",
                //    serviceKey,
                //    m_sAppName);

                // Get service object
                object oService =
                    m_oServicesCache.GetCached(serviceKey);

                // Fire pre-deactivate event
                FirePreServiceDeactivate(serviceKey, oService);

                if (oService != null)
                {
                    PerformReleaseService(
                        serviceKey);

                    PerformDisposeService(
                        oService);
                }
                else
                {
                    Log.Write(
                        Log.Severity.ERROR,
                        System.Reflection.MethodInfo.GetCurrentMethod(),
                        "Service [{0}] (null) in cache",
                        serviceKey
                        );
                }

                // Fire post-deactivate event
                FirePostServiceDeactivate(serviceKey);
            }

            // Clear the cache
            m_oServicesCache.OnCacheMiss -=
                new CacheMissEvent(ServicesCache_OnCacheMiss);

            m_oServicesCache.RevokeAll();

            // Clear load order 
            m_oServicesLoadOrder.Clear();

            m_oServicesActivator.Dispose();

            m_bDisposing = false;
        }

		#endregion

		//
		//	Protected 
		//
		
		protected object ServicesCache_OnCacheMiss(
			object				oSender, 
			CacheMissEventArgs	oArgs)
		{
			TisServiceKey oServiceKey = (TisServiceKey)oArgs.Key;

            //Log.Write(
            //    Log.Severity.DETAILED_DEBUG,
            //    System.Reflection.MethodInfo.GetCurrentMethod(),
            //    "Activating service [{0}] in App [{1}], ServiceCache: {2}",
            //    oServiceKey,
            //    m_sAppName,
            //    GetHashCode());

			// Return the service object from real service provider
			object oObj = m_oServicesActivator.GetService(oServiceKey);
			
			if(m_oServicesCache.GetCached(oServiceKey) != null)
			{
				return null;
			}

			// Fire pre-activate event
			FirePreServiceActivate(oServiceKey);

            if (!m_bDisposing)
            {
                // Keep load order
                UpdateLoadOrder(oServiceKey);
            }

			// Fire post-activate event
			FirePostServiceActivate(oServiceKey, oObj);

			return oObj;
		}

        private void UpdateLoadOrder(TisServiceKey oServiceKey)
        {
            if (m_oServicesLoadOrder.IndexOf(oServiceKey) < 0)
            {
                // Keep load order
                m_oServicesLoadOrder.Add(oServiceKey);
            }
        }

		private void PerformDisposeService(object oService)
		{
			try
			{
				if(oService is IDisposable)
				{
					((IDisposable)oService ).Dispose();
				}
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}
		}

		private void PerformReleaseService(
			TisServiceKey oServiceKey)
		{
			try
			{
                m_oServicesCache.Revoke(oServiceKey);

                m_oServicesActivator.ReleaseService(
					oServiceKey);
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}
		}


		#region Events helpers

		private void FirePreServiceActivate(TisServiceKey oServiceKey)
		{
			try
			{
				if(OnPreServiceActivate != null)
				{
					OnPreServiceActivate(m_sAppName, oServiceKey);
				}
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}
		}

		private void FirePostServiceActivate(TisServiceKey oServiceKey, object oService)
		{
			try
			{
				if(OnPostServiceActivate != null)
				{
					OnPostServiceActivate(m_sAppName, oServiceKey, oService);
				}
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}
		}

		private void FirePreServiceDeactivate(TisServiceKey oServiceKey, object oService)
		{
			try
			{
				if(OnPreServiceDeactivate != null)
				{
					OnPreServiceDeactivate(m_sAppName, oServiceKey, oService);
				}
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}
		}

		private void FirePostServiceDeactivate(TisServiceKey oServiceKey)
		{
			try
			{
				if(OnPostServiceDeactivate != null)
				{
					OnPostServiceDeactivate(m_sAppName, oServiceKey);
				}
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}
		}

		#endregion
	}
}
