using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Cache
{
	public interface IDataCache
	{
        TimeSpan RetryInterval { get; set; }

        int RetryAttempts { get; set; }

        void ActivateCache(string hostName, int portNumber, string applicationName);

		/// <summary>
		/// Get the item and add it if it does not exist
		/// </summary>
		/// <param name="key"></param>
		/// <param name="valueFactory"></param>
		/// <returns></returns>
		object GetOrAdd(
		   string key,
		   Func<string, object> valueFactory);
		/// <summary>
		/// Returns Cache Item - NULL if value does not exist
		/// </summary>
		object GetCacheItem(string key);
		/// <summary>
		/// Update item
		/// </summary>
		void PutCacheItem(string key, object value);
		/// <summary>
		/// If item does not exist, it will be added
		/// </summary>
		void PutCacheItem(string key, object value, bool addNew);
		/// <summary>
		/// Adds object in cache
		/// </summary>
		void AddCacheItem(string key, object value);
		/// <summary>
		/// Tries to add item to cache and returns status of the last operation
		/// </summary>
		bool TryAddCacheItem(string key, object value);
		/// <summary>
		/// Checks if cache contains given key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool TryGetCacheItem(string key, out object value);
		/// <summary>
		/// Removes given key and associated object from the cache.
		/// </summary>
		bool RemoveCacheItem(string key);
        
		/// <summary>
		/// Returns locked cache item
		/// </summary>
		object GetAndLockCacheItem(string key);
		/// <summary>
		/// Returns locked cache item
		/// </summary>
		object GetAndLockCacheItem(string key, TimeSpan timeToWait);
		/// <summary>
		/// Put and unlock cache item
		/// </summary>
		void PutAndUnlockCacheItem(string key, object value);
		/// <summary>
		/// Unlocks given key
		/// </summary>
		/// <param name="key"></param>
		void Unlock(string key);
		/// <summary>
		/// Safe unlock 
		/// </summary>
		bool TryUnlock(string key);

	}
}
