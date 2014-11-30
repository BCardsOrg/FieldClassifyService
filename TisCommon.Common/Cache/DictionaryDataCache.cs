using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;

namespace TiS.Core.TisCommon.Cache
{
    public delegate void ItemChangeDelegate(string key, DateTime changeDate);

	public class DictionaryDataCache : IDataCache
    {
		public event ItemChangeDelegate ItemChange;

        #region Private fields
		private ConcurrentDictionary<string, object> m_Cache = new ConcurrentDictionary<string, object>();
		private ConcurrentDictionary<string, object> m_LockedItems = new ConcurrentDictionary<string, object>();
		private ConcurrentDictionary<string, DateTime> m_CacheUpdate = new ConcurrentDictionary<string, DateTime>();

		private int m_WaitForLockedInterval = 100;

        private TimeSpan m_RetryInterval = TimeSpan.FromMilliseconds(0);
        private int m_RetryAttempts = 0;
        #endregion

		private void OnItemChange(string key, bool newItem)
		{
			DateTime changeDate = DateTime.Now;
			m_CacheUpdate.AddOrUpdate(key, changeDate, (k1, v1) => DateTime.Now);
			// Fire event if item was change - if it is new , then we do not need to tell it, because we assume it was never change before
			if ((ItemChange != null) && !newItem)
				ItemChange(key, changeDate);
		}
       
	#region IDataCache Members
		public object GetOrAdd(
		   string key,
		   Func<string, object> valueFactory)
		{
			return m_Cache.GetOrAdd(key, k =>
			{
				OnItemChange(k, true);
				return valueFactory(k); 
			}); 
		}
		/// <summary>
        /// Returns Cache Item - NULL if value does not exist
        /// </summary>
        public object GetCacheItem(string key)
        {
            lock (m_Cache)
            {
                if (m_Cache.ContainsKey(key) == true)
                {
                    return m_Cache[key];
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Update item
        /// </summary>
        public void PutCacheItem(string key, object value)
        {
            PutCacheItem(key, value, false); 
        }
        /// <summary>
        /// Update item
        /// </summary>
        public void PutCacheItem(string key, object value, bool addNew)
        {
			m_Cache.AddOrUpdate(key, value, (k, oldValue) => 
				{ 
					OnItemChange(key, oldValue == null);
					return value; 
				});
		}
        /// <summary>
        /// Adds object in cache
        /// </summary>
        public void AddCacheItem(string key, object value)
        {
			PutCacheItem(key, value, false);
		}
        /// <summary>
        /// Tries to add item to cache and returns status of the last operation
        /// </summary>
        public bool TryAddCacheItem(string key, object value)
        {
            try
            {
				AddCacheItem(key, value);
				return true;
            }
            catch (Exception e)
            {
                Log.WriteException(e);
                return false;
            }
        }
        /// <summary>
        /// Checks if cache contains given key
        /// </summary>
        public bool TryGetCacheItem(string key, out object value)
        {
            try
            {
                value = GetCacheItem(key);
                if (value == null) { return false; }

                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
        /// <summary>
        /// Removes given key and associated object from the cache
        /// </summary>
        /// <param name="key"></param>
        public bool RemoveCacheItem(string key)
        {
			Unlock(key);
			DateTime dateItem;
			m_CacheUpdate.TryRemove(key, out dateItem);
			object item;
            var result = m_Cache.TryRemove(key, out item);
            return result;
        }
        /// <summary>
        /// Locks object by key
        /// </summary>
        public object GetAndLockCacheItem(string key) 
        {
            return GetAndLockCacheItem(key, TimeSpan.MaxValue); 
        }
        /// <summary>
        /// Get and locks cache items
        /// </summary>
        public object GetAndLockCacheItem(string key, TimeSpan waitForLocked)
        {
            while (waitForLocked.TotalMilliseconds > 0)
            {
				lock (m_Cache)
				{
					if (m_LockedItems.ContainsKey(key) == false)
					{
                        m_LockedItems.AddOrUpdate(key, key, (k, v) => key);
						try
						{
							return m_Cache[key];
						}
						catch (Exception e)
						{
							object item;
							m_LockedItems.TryRemove(key, out item);
							throw e;
						}
					}
				}
                
                Thread.Sleep(m_WaitForLockedInterval);
                waitForLocked = waitForLocked.Subtract(TimeSpan.FromMilliseconds(m_WaitForLockedInterval));
            }
            throw new TimeoutException("Timeout Exception is occurred"); 
        }

        public void PutAndUnlockCacheItem(string key, object value)
        {
            lock (m_Cache)
            {
				object item;
				m_LockedItems.TryRemove(key, out item);
				AddCacheItem(key, value);
			}
        }

        public void Unlock(string key)
        {
			object item;
			m_LockedItems.TryRemove(key, out item);
        }

        public bool TryUnlock(string key)
        {
            try
            {
                Unlock(key);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public void ActivateCache(string hostName, int portNumber, string applicationName)
        {
        }

        #endregion

		public ConcurrentDictionary<string, DateTime> LastUpdateList
		{
			get
			{
				return m_CacheUpdate;
			}
		}

		public IEnumerable<string> Keys
		{
			get
			{
				return m_Cache.Keys;
			}
		}

		public void Clear()
		{
			foreach (var key in m_Cache.Keys.ToArray())
			{
				RemoveCacheItem(key);		 
			}
		}


        public TimeSpan RetryInterval
        {
            get
            {
                return m_RetryInterval;
            }
            set
            {
                m_RetryInterval = value;
            }
        }

        public int RetryAttempts
        {
            get
            {
                return m_RetryAttempts;    
            }
            set
            {
                m_RetryAttempts = value;
            }
        }
    }
}
