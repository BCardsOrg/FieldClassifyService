using System;

namespace TiS.Core.TisCommon.Customizations
{
	internal class BaseMethodInvoker : IDisposable
	{
		private BaseCache m_oInstanceCache;
		private BaseCache m_oMethodCache;

		public BaseMethodInvoker (BaseCache oMethodCache, BaseCache oInstanceCache)
		{
			if (oMethodCache != null)
			{
				m_oMethodCache   = oMethodCache;
                m_oMethodCache.Init(this);
			}

			if (oInstanceCache != null)
			{
				m_oInstanceCache = oInstanceCache;
                m_oInstanceCache.Init(this);
			}
		}

		protected virtual object GetInstance (
			params object[] Args)
		{
			return GetObject (
				m_oInstanceCache, 
				Args);
		}

		protected virtual object GetMethod (
			params object[] Args) 
		{
			return GetObject (
				m_oMethodCache, 
				Args);
		}

		protected virtual object GetObject (
			BaseCache oCache, 
			params object[] Args)
		{
			return oCache.Get (oCache.ObtainPersistKey (Args));
		}

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
