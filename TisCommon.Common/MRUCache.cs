using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Threading;
using System.Collections.Concurrent;

namespace TiS.Core.TisCommon
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public class CacheMissEventArgs: EventArgs
	{
		private object m_oKey;

		public CacheMissEventArgs(object oKey)
		{
			m_oKey = oKey;
		}

		public object Key
		{
			get { return m_oKey; }
		}
	}

	[System.Runtime.InteropServices.ComVisible(false)]
	public class CacheRevokeEventArgs: EventArgs
	{
		public enum Timing { Pre, Post }

		private object m_oKey;
		private object m_oValue;
		private Timing m_enTiming;
				

		public CacheRevokeEventArgs(object oKey, object oVal, Timing enTiming)
		{
			m_oKey   = oKey;
			m_oValue = oVal;
			m_enTiming = enTiming;
		}

		public object Key
		{
			get { return m_oKey; }
		}

		public object Value
		{
			get { return m_oValue; }
		}

		public Timing EventTiming
		{
			get { return m_enTiming; }
		}
	}

	public delegate object CacheMissEvent(
		object				oSender, 
		CacheMissEventArgs	oArgs);

	public delegate void CacheRevokeEvent(
		object				    oSender, 
		CacheRevokeEventArgs	oArgs);

	public interface ICache
	{
		event CacheMissEvent   OnCacheMiss;
		event CacheRevokeEvent OnCacheRevoke;

		object Get(object oKey);
		object GetCached(object oKey);
		void   Put(object oKey, object oValue);
		void   Revoke(object oKey);
		void   RevokeAll();
        bool   IsInCache(object oKey);
	}


	[Serializable]
	[KnownType(typeof(System.Collections.Specialized.HybridDictionary))]
	public class MRUCache : ICache, IDisposable
	{
        [DataMember]
        private ConcurrentDictionary<object, object> m_oCachedData = new ConcurrentDictionary<object, object>();

		[DataMember]
		private bool m_bDisposeOnRevoke = true;

		//
		//	Public 
		//


		[field: DataMember]
		public event CacheMissEvent OnCacheMiss;
		[field: DataMember]
		public event CacheRevokeEvent OnCacheRevoke;
		
		public MRUCache()
			:this(true, null)
		{
		}

		public MRUCache(bool bDisposeOnRevoke)
			:this(bDisposeOnRevoke, null)
		{
		}

		public MRUCache(bool bDisposeOnRevoke, ConcurrentDictionary<object, object> oDictionary)
		{
			m_bDisposeOnRevoke = bDisposeOnRevoke;	

			if(oDictionary == null)
			{
                m_oCachedData = new ConcurrentDictionary<object, object>();
			}
			else
			{
				m_oCachedData = oDictionary;
			}
		}
		
		#region ICache implementation

        public object Get(object oKey)
        {
            object oResult = null;

            // Try to get the data from cache
            oResult = GetFromCache(oKey);

            // If not found
            if (oResult == null)
            {
                // Allow fill it
                OnCacheMissImpl(oKey);

                // Re-query the cache
                oResult = GetFromCache(oKey);
            }

            // Return result (may be null)
            return oResult;
        }

		public object GetCached(object oKey)
		{
			return GetFromCache(oKey);
		}

        public void Put(object oKey, object oValue)
        {
            AddToCache(oKey, oValue);
        }

        public void Revoke(object oKey)
        {
            object oVal = GetFromCache(oKey);

            // Fire OnCacheRevoke event
            if (OnCacheRevoke != null)
            {
                OnCacheRevoke(
                    this,
                    new CacheRevokeEventArgs(oKey, oVal, CacheRevokeEventArgs.Timing.Pre));
            }

            if (m_bDisposeOnRevoke)
            {
                // Get as IDisposable
                IDisposable oDisposableObj =
                    oVal as IDisposable;

                if (oDisposableObj != null)
                {
                    // Dispose object
                    oDisposableObj.Dispose();
                }
            }

            object value;
            m_oCachedData.TryRemove(oKey, out value);

            // Fire OnCacheRevoke event
            if (OnCacheRevoke != null)
            {
                OnCacheRevoke(
                    this,
                    new CacheRevokeEventArgs(oKey, oVal, CacheRevokeEventArgs.Timing.Post));
            }
        }

        public void RevokeAll()
        {
            // Create copy keys array
            object[] Keys = GetCachedKeys();

            // Revoke all objects
            foreach (object oKey in Keys)
            {
                Revoke(oKey);
            }
        }

		#endregion

        public object[] GetCachedKeys()
        {
            // Create copy keys array
            object[] Keys =
                new object[m_oCachedData.Keys.Count];

            m_oCachedData.Keys.CopyTo(Keys, 0);

            return Keys;
        }

        public bool IsInCache(object oKey)
        {
            return m_oCachedData.ContainsKey(oKey);
        }

        //
		//	Protected methods
		//

		// Template method
		protected virtual object GetValue(object oKey)
		{
			return null;
		}
		
		//
		//	Private methods
		//

		private void OnCacheMissImpl(object oKey)
		{
			object oValue = null;
			
			// Call template method first
			oValue = GetValue(oKey);

			// If template method returned null
			if(oValue == null)
			{
				// If event defined 
				if(OnCacheMiss != null)
				{
					// Fire event
					oValue = OnCacheMiss(this, new CacheMissEventArgs(oKey));
				}
			}

			// If value retreived
			if(oValue != null)
			{
				// Add the value to cache
				AddToCache(oKey, oValue);
			}

		}

        private void AddToCache(object oKey, object oValue)
        {
            m_oCachedData.TryAdd(oKey, oValue);
        }

        private object GetFromCache(object oKey)
        {
            object value;
            m_oCachedData.TryGetValue(oKey, out value);
            return value;
        }


        #region IDisposable Members

        public virtual void Dispose()
        {
            if (OnCacheMiss != null)
            {
                OnCacheMiss = null;
            }

            foreach (object cachedObject in m_oCachedData.Values)
            {
                if (cachedObject is IDisposable)
                {
                    (cachedObject as IDisposable).Dispose();
                }
            }

            m_oCachedData.Clear();
        }

        #endregion
    }
}
