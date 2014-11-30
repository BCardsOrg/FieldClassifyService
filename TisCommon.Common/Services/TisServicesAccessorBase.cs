using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Configuration;
using TiS.Core.TisCommon.DataModel;
using TiS.Core.TisCommon.Attachment.File;
using TiS.Core.TisCommon.FilePath;
using TiS.Core.TisCommon.Security;
using TiS.Core.TisCommon.Customizations;
using TiS.Core.TisCommon.Validation;

namespace TiS.Core.TisCommon.Services
{
	[ComVisible(false)]
	public class TisServicesAccessorBase : IDisposable
	{
        TisApplicationServicesProvider m_oAppServiceProvider;

        public TisServicesAccessorBase(TisApplicationServicesProvider oAppServiceProvider)
		{
			if(oAppServiceProvider == null)
			{
				ExceptionUtil.RaiseArgumentNullException(
					"oAppServiceProvider",
					"can't be null",
					System.Reflection.MethodInfo.GetCurrentMethod());
			}

			m_oAppServiceProvider = oAppServiceProvider;
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (m_oAppServiceProvider != null)
            {
                m_oAppServiceProvider.Dispose();
                m_oAppServiceProvider = null;
            }
        }

        #endregion

        protected string ApplicationName
        {
            get
            {
                return m_oAppServiceProvider.ApplicationName;
            }
        }

        protected TisApplicationServicesProvider AppServiceProvider
        {
            get { return m_oAppServiceProvider; }
        }

        public BasicConfiguration BasicConfiguration
        {
            get
            {
                return (BasicConfiguration)GetService(TisServicesSchema.BasicConfiguration);
            }
        }

        public ITisServiceRegistry ServiceRegistry
        {
            get
            {
                return GetServiceRegistry();
            }
        }

        public TisEntityReflection GetEntityReflection(string instanceName)
        {
            return (TisEntityReflection)GetService(TisServicesSchema.EntityReflection, instanceName);
        }

        public LocalPathLocator LocalPathLocator
        {
            get
            {
                return (LocalPathLocator)GetService(TisServicesSchema.LocalPathLocator);
            }
        }

        public LocalPathProvider LocalPathProvider
        {
            get
            {
                return (LocalPathProvider)GetService(TisServicesSchema.LocalPathProvider);
            }
        }

        public TisValidationPolicy ValidationPolicy
        {
            get
            {
                return (TisValidationPolicy)GetService(TisServicesSchema.ValidationPolicy);
            }
        }

        public virtual TisSecurityMngr SecurityManager
        {
            get
            {
                return null;
            }
        }

        public virtual ITisEventsManager EventsManager
        {
            get
            {
                return null;
            }
        }

        public virtual TisAttachedFileManager SetupAttachmentsFileManager
        {
            get
            {
                return (TisAttachedFileManager)GetService(TisServicesSchema.SetupAttachmentsFileManager);
            }
        }

        public virtual ISpecificConfigurationService ConfigurationManager
        {
            get
            {
                return null;
            }
        }

        public object GetService(Type oType)
		{
			return m_oAppServiceProvider.GetService(oType);
		}

        public object GetService(string sServiceName)
		{
			return m_oAppServiceProvider.GetService(sServiceName);
		}

        public object GetService(
			Type	oType, 
			string	sInstanceName)
		{
			return m_oAppServiceProvider.GetService(oType, sInstanceName);
		}

        public object GetService(
			string sServiceName, 
			string sInstanceName)
		{
			return m_oAppServiceProvider.GetService(sServiceName, sInstanceName);
		}

        public object GetService(
			ITisServiceInfo oServiceInfo)
		{
			return GetService(oServiceInfo.ServiceName);
		}

        public object GetService(
			ITisServiceInfo oServiceInfo,
			string			sInstanceName)
		{
			return GetService(
				oServiceInfo.ServiceName,
				sInstanceName
				);
		}

        protected ITisServiceRegistry GetServiceRegistry()
        {
            return m_oAppServiceProvider.GetServiceRegistry();
        }

        public bool IsServiceInstalled(string sServiceName)
        {
            return m_oAppServiceProvider.IsServiceInstalled(sServiceName);
        }

        public bool IsServiceInstalled(Type oServiceType)
        {
            return m_oAppServiceProvider.IsServiceInstalled(oServiceType);
        }
    }
}
