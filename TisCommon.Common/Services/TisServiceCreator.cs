using System;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Configuration;
using TiS.Core.TisCommon.Security;

namespace TiS.Core.TisCommon.Services
{
    #region TisServiceCreatorContext

    [ComVisible(false)]
	public class TisServiceCreatorContext : IDisposable
	{
        private TisPermissionValidatorContext m_permissionValidatorContext;
        private ITisSecurityMngr m_securityMngr;
        private ITisServiceSecurityCheck m_serviceSecurityCheck;
		
		public TisServiceCreatorContext(
			string			applicationName,
			TisServiceKey	serviceKey,
			ITisServicesHost servicesHost)
		{
            ApplicationName = applicationName;
            ServiceKey = serviceKey;
            ServicesHost = servicesHost;
		}

        #region IDisposable Members

        public void Dispose()
        {
            ApplicationName = null;
            ServiceKey = null;
            m_permissionValidatorContext = null;
            m_securityMngr = null;
            m_serviceSecurityCheck = null;
        }

        #endregion

        public string ApplicationName { get; private set; }
        public TisServiceKey ServiceKey { get; private set; }
		public ITisServicesHost ServicesHost { get; private set; }

        public BasicConfiguration BasicConfiguration
        {
            get
            {
                return ServicesHost.BasicConfiguration;
            }
        }

		#region GetService
		 
		public object GetService(string sServiceName)
		{
            return ServicesHost.GetService(
				ApplicationName, 
				sServiceName);
		}

		public object GetService(Type oServiceType)
		{
            return ServicesHost.GetService(
				ApplicationName, 
				oServiceType);
		}

		public object GetService(string sServiceName, string sInstanceName)
		{
            return ServicesHost.GetService(
				ApplicationName, 
				sServiceName,
				sInstanceName);
		}

		public object GetService(Type oServiceType, string sInstanceName)
		{
            return ServicesHost.GetService(
				ApplicationName, 
				oServiceType,
				sInstanceName);
		}

        public object GetService(ITisServiceInfo serviceInfo)
        {
            return ServicesHost.GetService(
                ApplicationName,
                serviceInfo.ServiceName);
        }


        public object GetService(ITisServiceInfo serviceInfo, string sInstanceName)
        {
            return ServicesHost.GetService(
                ApplicationName,
                serviceInfo.ServiceName,
                sInstanceName);
        }

        #endregion

		#region GetSystemService

		public object GetSystemService(string sServiceName)
		{
            return ServicesHost.GetSystemService(
				sServiceName);
		}

		public object GetSystemService(Type oServiceType)
		{
            return ServicesHost.GetSystemService(
				oServiceType);
		}

		public object GetSystemService(string sServiceName, string sInstanceName)
		{
            return ServicesHost.GetSystemService(
				sServiceName,
				sInstanceName);
		}

		public object GetSystemService(Type oServiceType, string sInstanceName)
		{
            return ServicesHost.GetSystemService(
				oServiceType,
				sInstanceName);
		}

		#endregion

		#region Security

        public ITisSecurityMngr SecurityMngr
        {
            get
            {
                if (m_securityMngr == null)
                {
                    m_securityMngr = (ITisSecurityMngr)ServicesHost.GetService(ApplicationName, "SecurityManager");
                }

                return m_securityMngr;
            }
        }

        public ITisSecurityCheck SecurityCheck
        {
            get
            {
                return (ITisSecurityCheck)SecurityMngr;
            }
        }

        public ITisServiceSecurityCheck ServiceSecurityCheck
        {
            get
            {
                if (m_serviceSecurityCheck == null)
                {
                    ITisServiceInfo oServiceInfo = ServicesHost.CheckedGetServiceInfo(
                        ApplicationName,
                        ServiceKey.ServiceName);

                    if (oServiceInfo != null)
                    {
                        m_serviceSecurityCheck = new TisServicesSecurityCheck(
                            oServiceInfo,
                            SecurityCheck);
                    }
                }

                return m_serviceSecurityCheck;
            }
        }

		#endregion

        public virtual TisPermissionValidatorContext PermissionValidatorContext
        {
            get
            {
                if (m_permissionValidatorContext == null)
                {
                    m_permissionValidatorContext =
                        new TisPermissionValidatorContext(ApplicationName,
                                                          ServicesHost);
                }

                return m_permissionValidatorContext;
            }
        }
    }

	#endregion

    #region TisServiceBaseCreatorContextSetter

    public abstract class TisServiceBaseCreatorContextSetter : ITisServiceCreatorContextSetter
    {
        private ITisServiceCreatorContextSetter m_oNext;

        public ITisServiceCreatorContextSetter Next
        {
            get { return m_oNext; }
            set { m_oNext = value; }
        }

        public virtual void SetCreatorContext(
            ITisServiceCreator oCreator,
            string sAppName,
            TisServiceKey oServiceKey,
            ITisServicesHost oServicesHost)
        {
            // Perform context set
            SetCreatorContextImpl(
                oCreator,
                sAppName,
                oServiceKey,
                oServicesHost);

            // If has next setter in the chain
            if (Next != null)
            {
                // Forward the request to next setter
                Next.SetCreatorContext(
                    oCreator,
                    sAppName,
                    oServiceKey,
                    oServicesHost);
            }
        }

        protected abstract void SetCreatorContextImpl(
            ITisServiceCreator oCreator,
            string sAppName,
            TisServiceKey oServiceKey,
            ITisServicesHost oServicesHost);
    }

    #endregion

    #region TisServiceCreatorContextSetter

    [ComVisible(false)]
    public class TisServiceCreatorContextSetter : TisServiceBaseCreatorContextSetter
    {
        public TisServiceCreatorContextSetter()
        {
        }

        protected override void SetCreatorContextImpl(
            ITisServiceCreator oCreator,
            string sAppName,
            TisServiceKey oServiceKey,
            ITisServicesHost oServicesHost)
        {
            // Query for ISupportsCreatorContext interface
            ISupportsCreatorContext oTarget =
                oCreator as ISupportsCreatorContext;

            if (oTarget != null)
            {
                // Set context
                oTarget.SetContext(
                    sAppName,
                    oServiceKey,
                    oServicesHost);
            }

        }
    }

    #endregion

    #region TisServiceCreator

    public abstract class TisServiceCreatorBase: 
		ISupportsCreatorContext, 
		ITisServiceCreator,
        IDisposable
	{
		private TisServiceCreatorContext m_oContext;
        private TisPermissionValidator m_permissionValidator;
        private TisServicePermissionValidator m_servicePermissionValidator;

		public TisServiceCreatorBase()
		{
		}

		public void SetContext(
			string				 sAppName,
			TisServiceKey			 oServiceKey,
			ITisServicesHost oServicesHost)
		{
			m_oContext = new TisServiceCreatorContext(
				sAppName,
				oServiceKey,
				oServicesHost);
		}

        #region IDisposable Members

        public void Dispose()
        {
            m_oContext.Dispose();

            m_permissionValidator = null;
            m_servicePermissionValidator = null;
        }

        #endregion

		#region ITisServiceCreator implementation

		public abstract object CreateService();

		public virtual void ReleaseService(
			object oService)
		{
		}

		#endregion

        protected string ApplicationName
		{
			get { return Context.ApplicationName; }
		}

		protected string NodeName
		{
			get { return Context.ServicesHost.Name; }
		}

		protected TisServiceCreatorContext Context
		{
			get { return m_oContext; }
		}

		protected ITisServicesHost Services
		{
			get { return m_oContext.ServicesHost; }
		}

        protected ITisSecurityCheck SecurityCheck
        {
            get
            {
                return m_oContext.SecurityCheck;
            }
        }

        protected TisPermissionValidator PermissionValidator
        {
            get
            {
                if (m_permissionValidator == null)
                {
                    m_permissionValidator = new TisPermissionValidator(Context.PermissionValidatorContext);
                }

                return m_permissionValidator;
            }
        }

        protected TisServicePermissionValidator ServicePermissionValidator
        {
            get
            {
                if (m_servicePermissionValidator == null)
                {
                    m_servicePermissionValidator = new TisServicePermissionValidator(
                        Context.ServicesHost.CheckedGetServiceInfo(Context.ApplicationName,
                                                                   Context.ServiceKey.ServiceName),
                        Context.PermissionValidatorContext);
                }

                return m_servicePermissionValidator;
            }
        }

        protected PermissionValidatorDelegate ServicePermissionValidatorDelegate
        {
            get
            {
                return new PermissionValidatorDelegate(ServicePermissionValidator.Validate);
            }
        }
    }
	#endregion
}
