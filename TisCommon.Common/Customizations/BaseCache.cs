using System;

namespace TiS.Core.TisCommon.Customizations
{
	internal class BaseCache : MRUCache, IDisposable
	{
		private const string PERSIST_KEY_DELIMITER = "$";
		private object m_oCacheCreator;

		public BaseCache ()
		{
            OnCacheMiss += new CacheMissEvent(OnCacheMissHandler);
        }

        internal void Init(object cacheCreator)
        {
            m_oCacheCreator = cacheCreator;
        }

        internal object CacheCreator
        {
            get
            {
                return m_oCacheCreator;
            }
        }

		internal virtual object ObtainPersistKey (params object[] Args)
		{
			ArrayBuilder oArrayBuilder = new ArrayBuilder (Args);
 
			string sPersistKey = String.Join (PERSIST_KEY_DELIMITER, (string[])oArrayBuilder.GetArray (typeof (string)));

			if (sPersistKey == String.Empty)
			{
				throw new TisException (
					"Invalid key");
			}

			return sPersistKey;
		}

		internal virtual string[] ParsePersistKey (object oPersistKey)
		{
			string[] ParsedPersitKey = ((string) oPersistKey).Split (PERSIST_KEY_DELIMITER [0]);

			if (!ValidatePersistKey (ParsedPersitKey))
			{
				throw new TisException (
					"Invalid key");
			}
 
            return ParsedPersitKey;  
		}

		internal virtual bool ValidatePersistKey (string[] ParsedPersitKey)
		{
			return false;
		}

		internal virtual object OnCacheMissHandler(object oSender, CacheMissEventArgs oArgs)
		{
			return null;
		}

        #region IDisposable Members

        public override void Dispose()
        {
            OnCacheMiss -= new CacheMissEvent(OnCacheMissHandler);

            m_oCacheCreator = null;

            base.Dispose();
        }

        #endregion
    }
}
