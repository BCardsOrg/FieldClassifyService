using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Services
{
    [ComVisible(false)]
    public class TisServiceRegistryProvider : ITisServiceRegistryProvider
    {
        private TisServiceRegistry m_serviceRegistry;
        private GacAssemblyResolver m_gacAssemblyResolver;

        public TisServiceRegistryProvider()
        {
            m_gacAssemblyResolver = new GacAssemblyResolver();
            Type serviceRegistryProviderType = null;

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(m_gacAssemblyResolver.AssemblyResolveHandler);
                serviceRegistryProviderType = Type.GetType(TisServicesConst.SERVICE_REGISTRY_PROVIDER_TYPE_NAME);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(m_gacAssemblyResolver.AssemblyResolveHandler);
            }

            if (serviceRegistryProviderType == null)
            {
                throw new TisException("Failed to load services schema ");
            }

            m_serviceRegistry = new TisServiceRegistry(serviceRegistryProviderType);
        }

        #region IServiceRegistryProvider

        public ITisServiceRegistry GetServiceRegistry(string sAppName)
        {
            return m_serviceRegistry;
        }

        #endregion
    }
}
