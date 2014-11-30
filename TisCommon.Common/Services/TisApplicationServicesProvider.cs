using System;
using System.Reflection;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.Services
{
	public class TisApplicationServicesProvider: IAppServiceProvider
	{
		public TisApplicationServicesProvider(
            ITisServicesHost oServicesHost,
			string		  sAppName)
		{
			if(oServicesHost == null)
			{
				ExceptionUtil.RaiseArgumentNullException(
					"oServicesHost",
					"can't be null",
					MethodInfo.GetCurrentMethod());
			}

			if(!StringUtil.IsStringInitialized(sAppName))
			{
				ExceptionUtil.RaiseArgumentNullException(
					"sAppName",
					"Must be initialized",
					MethodInfo.GetCurrentMethod());
			}

			ServicesHost = oServicesHost;
			ApplicationName      = sAppName;
		}

        public ITisServicesHost ServicesHost { get; private set; }

        public string ApplicationName { get; private set; }

        public object GetService(Type oType)
		{
			return ServicesHost.GetService(
				ApplicationName,
				oType
				);
		}

		public object GetService(string sServiceName)
		{
			return ServicesHost.GetService(
				ApplicationName,
				sServiceName
				);
		}

		public object GetService(
			Type	oType, 
			string	sInstanceName)
		{
			return ServicesHost.GetService(
				ApplicationName,
				oType,
				sInstanceName
				);
		}

		public object GetService(
			string sServiceName, 
			string sInstanceName)
		{
			return ServicesHost.GetService(
				ApplicationName,
				sServiceName,
				sInstanceName
				);
		}

        public object GetService(ITisServiceInfo serviceInfo)
        {
            return ServicesHost.GetService(
                ApplicationName,
                serviceInfo.ServiceName);
        }

        public object GetService(
            ITisServiceInfo serviceInfo,
            string sInstanceName)
        {
            return ServicesHost.GetService(
                ApplicationName,
                serviceInfo.ServiceName,
                sInstanceName);
        }

        public ITisServiceRegistry GetServiceRegistry()
        {
            return ServicesHost.GetServiceRegistry(ApplicationName);
        }

        public ITisServiceInfo GetServiceInfo(string sServiceName)
        {
            return ServicesHost.GetServiceInfo(ApplicationName, sServiceName);
        }

        public bool IsServiceInstalled(string sServiceName)
        {
            return ServicesHost.IsServiceInstalled(ApplicationName, sServiceName);
        }

        public bool IsServiceInstalled(Type oServiceType)
        {
            return ServicesHost.IsServiceInstalled(ApplicationName, oServiceType);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
