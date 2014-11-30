using System;

namespace TiS.Core.TisCommon.Services
{
    #region ServiceProviderFactory

    public class TisServiceProviderFactory : IDisposable
	{
		private TisServicesHost m_oNode;

		public TisServiceProviderFactory(TisServicesHost oServicesHost)
		{
			m_oNode = oServicesHost;
		}

        #region IDisposable Members

        public void Dispose()
        {
            m_oNode = null;
        }

        #endregion

        public ITisServiceProvider CreateServiceProvider(
			string sAppName)
		{
			// Create service activator
			TisServiceActivator oActivator = new TisServiceActivator(
				sAppName, 
				m_oNode);

			// Create service cache
            TisServiceCache oCache = new TisServiceCache(
				oActivator, 
				sAppName);
			
			// Return the cache 
			return oCache;
		}
    }

    #endregion 
}
