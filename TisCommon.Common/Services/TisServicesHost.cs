using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using TiS.Core.TisCommon.Reflection;
using TiS.Core.TisCommon.Network;
using TiS.Core.TisCommon.Configuration;
using TiS.Core.TisCommon.Customizations;
using TiS.Core.TisCommon.Services.Web;
using System.Threading;

namespace TiS.Core.TisCommon.Services
{
    #region TisServicesHost

    public class TisServicesHost : ITisServicesHost
    {
        private const string ALL_APPS = "";

        private HostStatus m_enStatus = HostStatus.Inactive;
        private static TisServiceRegistry m_serviceRegistry;
        private ITisServiceCreatorContextSetter m_oCreatorContextSetter;
        private TisServiceProviderCache m_oServiceProvidersCache;
        private ITisServiceCreatorContextSetter m_creatorContextSetter;
        private ReaderWriterLock m_hostStatusLocker = new ReaderWriterLock();

        protected enum HostStatus { Inactive, Initializing, Active }
        protected bool m_bDisposed = false;

        public event TisServiceEventDelegate OnPreServiceActivate;
        public event TisServiceEventDelegate OnPostServiceActivate;
        public event TisServiceEventDelegate OnPreServiceDeactivate;
        public event TisServiceEventDelegate OnPostServiceDeactivate;

        static TisServicesHost()
        {
            m_serviceRegistry = (TisServiceRegistry)new TisServiceRegistryProvider().GetServiceRegistry(ALL_APPS);
        }

        public TisServicesHost()
        {
            m_creatorContextSetter = new TisServiceCreatorContextSetter();

            IsRestrictedMode = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public string Name {get; private set;}

        public BasicConfiguration BasicConfiguration { get; private set; }

        public string StationName { get; set; }

		public string StationId { get; set; }

        public bool IsRestrictedMode { get; set; }

        public bool IsWebSession { get; set; }

        public TisWebSessionData WebSessionData { get; set; }

        public string UserName { get; set; }

        public virtual void StopApplicationServices(string sAppName)
        {
            if (!IsActive)
            {
                return;
            }

            if (StringUtil.IsStringInitialized(sAppName))
            {
                m_oServiceProvidersCache.Revoke(sAppName);
            }
            else
            {
                m_oServiceProvidersCache.RevokeAll();
            }
        }

        public string GetBuiltInServicesSchema(string sAppName)
        {
            return m_serviceRegistry.ServicesSchema;
        }


        #region Init

        public virtual void Initialize(BasicConfiguration basicConfiguration) 
        {
            Initialize(basicConfiguration.LocalMachineName, basicConfiguration);
        }

        public virtual void Initialize(
            string name,
            BasicConfiguration basicConfiguration)
        {
            SetStatus(HostStatus.Initializing);

            Name            = name;
            BasicConfiguration = basicConfiguration;

            AddCreatorContextSetter(m_creatorContextSetter);

            InitServiceProvidersCache();

            SetStatus(HostStatus.Active);
        }

        #endregion

        protected bool IsServer
        {
            get
            {
                return StringUtil.CompareIgnoreCase(BasicConfiguration.ServerMachineName, Name);
            }
        }


        #region CreatorContextSetter

        public ITisServiceCreatorContextSetter CreatorContextSetter
        {
            get { return m_oCreatorContextSetter; }
        }

        public void AddCreatorContextSetter(
            ITisServiceCreatorContextSetter oCreatorContextSetter)
        {
            // Add ICreatorContextSetter to chain

            // If first
            if (m_oCreatorContextSetter == null)
            {
                // Set the member
                m_oCreatorContextSetter = oCreatorContextSetter;
            }
            else
            {
                ITisServiceCreatorContextSetter oPrev = m_oCreatorContextSetter;

                // Find the last in chain
                while (oPrev.Next != null)
                {
                    oPrev = oPrev.Next;
                }

                // Add to end of the chain
                oPrev.Next = oCreatorContextSetter;
            }
        }

        #endregion

        #region GetService

        #region Full location specification

        public object GetService(
            string sHostName,
            string sAppName,
            string sServiceName,
            string sServiceInstanceName)
        {
            return GetServiceImpl(
                sHostName,
                sAppName,
                sServiceName,
                sServiceInstanceName);
        }

        public object GetService(
            string sHostName,
            string sAppName,
            Type oServiceType,
            string sServiceInstanceName)
        {
            return GetServiceImpl(
                sHostName,
                sAppName,
                ServiceNameFromServiceType(oServiceType),
                sServiceInstanceName);
        }

        #endregion

        #region ServicesHost not specified

        public object GetService(
            string sAppName,
            string sServiceName)
        {
            return GetService(
                sAppName,
                sServiceName,
                TisServicesConst.UNNAMED_INSTANCE);
        }

        public object GetService(
            string sAppName,
            Type oServiceType)
        {
            return GetService(
                sAppName,
                ServiceNameFromServiceType(oServiceType));
        }

        public object GetService(
            string sAppName,
            Type oServiceType,
            string sServiceInstanceName)
        {
            return GetService(
                sAppName,
                ServiceNameFromServiceType(oServiceType),
                sServiceInstanceName);
        }

        public object GetService(
            string sAppName,
            string sServiceName,
            string sServiceInstanceName)
        {
            // Select ServicesHost for the service
            string sServicesHost = SelectServicesHost(
                sAppName,
                sServiceName);

            // Call GetService with ServicesHost specified
            return GetService(
                sServicesHost,
                sAppName,
                sServiceName,
                sServiceInstanceName);
        }

        #endregion

        #endregion

        #region GetSystemService

        public object GetSystemService(
            string sServiceName,
            string sServiceInstanceName)
        {
            return GetService(
                TisServicesConst.SystemApplication,
                sServiceName,
                sServiceInstanceName);
        }

        public object GetSystemService(
            Type oServiceType,
            string sServiceInstanceName)
        {
            return GetSystemService(
                ServiceNameFromServiceType(oServiceType),
                sServiceInstanceName);
        }

        public object GetSystemService(string sServiceName)
        {
            return GetSystemService(
                sServiceName,
                TisServicesConst.UNNAMED_INSTANCE);
        }

        public object GetSystemService(Type oServiceType)
        {
            return GetSystemService(
                ServiceNameFromServiceType(oServiceType));
        }

        #endregion

        #region CanHostService

        public bool CanHostService(
            string sAppName,
            string sServiceName)
        {
            // Obtain service info
            ITisServiceInfo oServiceInfo =
                CheckedGetServiceInfo(sAppName, sServiceName);

            // Check if can host the service
            return CanHostService(
                sAppName,
                oServiceInfo);
        }

        public bool CanHostService(
            string sAppName,
            ITisServiceInfo oServiceInfo)
        {
            // Optimization - doesn't check ServicesHost roles if
            // service doesn't requires any role
            if (oServiceInfo.RequiredRoles.Length == 0)
            {
                return true;
            }

            bool canHost = (Array.IndexOf(oServiceInfo.RequiredRoles, TisServicesConst.SERVER_ROLE) >= 0 &&
                             StringUtil.CompareIgnoreCase(Name, BasicConfiguration.ServerMachineName));

            return canHost;
        }

        public bool CanHostService(
            string hostName,
            string sAppName,
            ITisServiceInfo oServiceInfo)
        {
            if (oServiceInfo.RequiredRoles.Length == 0)
            {
                return true;
            }

            bool canHost = (Array.IndexOf(oServiceInfo.RequiredRoles, TisServicesConst.SERVER_ROLE) >= 0 &&
                            StringUtil.CompareIgnoreCase(hostName, BasicConfiguration.ServerMachineName));

            return canHost;
        }

        #endregion

        #region ServiceRegistry/ServiceInfo access

        public ITisServiceRegistry GetServiceRegistry(
            string sAppName)
        {
            return m_serviceRegistry;
        }

        public ITisServiceInfo GetServiceInfo(
            string sAppName,
            string sServiceName)
        {
            // Get service registry
            ITisServiceRegistry oServiceRegistry =
                GetServiceRegistry(sAppName);

            // Obtain service info
            ITisServiceInfo oServiceInfo =
                oServiceRegistry.GetInstalledServiceInfo(sServiceName);

            return oServiceInfo;
        }

        public ITisServiceInfo CheckedGetServiceInfo(
            string sAppName,
            string sServiceName)
        {
            // Get service info
            ITisServiceInfo oServiceInfo = GetServiceInfo(
                sAppName,
                sServiceName);

            // Validate
            if (oServiceInfo == null)
            {
                throw new TisException(
                    "Service [{0}] not installed in application [{1}]",
                    sServiceName,
                    sAppName);
            }

            return oServiceInfo;
        }

        #endregion

        #region IsServiceInstalled

        public bool IsServiceInstalled(
            string sAppName,
            string sServiceName)
        {
            return (GetServiceInfo(
                sAppName,
                sServiceName) != null);
        }

        public bool IsServiceInstalled(
            string sAppName,
            Type oServiceType)
        {
            return (GetServiceInfo(
                sAppName,
                ServiceNameFromServiceType(oServiceType)) != null);
        }

        #endregion

        public void ReleaseService(
            string applicationName,
            string serviceName,
            string serviceInstanceName = TisServicesConst.UNNAMED_INSTANCE)
        {
            ITisServiceProvider serviceProvider = m_oServiceProvidersCache.GetServiceProvider(applicationName);

            ITisServiceInfo serviceInfo =
                CheckedGetServiceInfo(applicationName, serviceName);

            TisServiceKey serviceKey = new TisServiceKey(
                serviceName,
                serviceInstanceName);

            serviceProvider.ReleaseService(serviceKey, serviceInfo);
        }

        public bool AddLocalService(
            string applicationName,
            string serviceName,
            object serviceInstance = null,
            string serviceCreatorFullName = null,
            string serviceTypeFullName = null,
            string serviceInstanceName = null)
        {
            if (!StringUtil.IsStringInitialized(serviceName))
            {
                Log.WriteWarning("Try to install service without name.");
                return false;
            }

            ITisServiceRegistry applicationServiceRegistry = GetServiceRegistry(applicationName);

            bool isServiceInstalled = applicationServiceRegistry.IsServiceInstalled(serviceName);

            if (!isServiceInstalled)
            {
                if (!StringUtil.IsStringInitialized(serviceCreatorFullName))
                {
                    serviceCreatorFullName =
                        typeof(TisUniversalServiceCreator).FullName + "," +
                        typeof(TisUniversalServiceCreator).Assembly.GetName().Name;
                }

                if (!StringUtil.IsStringInitialized(serviceTypeFullName))
                {
                    serviceTypeFullName = String.Empty;
                }

                try
                {
                    applicationServiceRegistry.InstallService(
                        serviceName,
                        serviceCreatorFullName,
                        serviceTypeFullName,
                        TisServicesConst.NO_ROLES_REQUIRED,
                        false);
                }
                catch (Exception exc)
                {
                    Log.WriteWarning("Failed to install service [{0}]. Details : {1}", serviceName, exc.Message);
                }

                isServiceInstalled = applicationServiceRegistry.IsServiceInstalled(serviceName);
            }

            if (serviceInstance != null)
            {
                if (!StringUtil.IsStringInitialized(serviceInstanceName))
                {
                    serviceInstanceName = TisServicesConst.UNNAMED_INSTANCE;
                }

                TisServiceKey serviceKey = new TisServiceKey(
                    serviceName,
                    serviceInstanceName);

                ITisServiceProvider serviceProvider;

                serviceProvider = m_oServiceProvidersCache.GetServiceProvider(applicationName);

                serviceProvider.AddService(serviceKey, serviceInstance);

                ITisServiceInfo serviceInfo =
                    CheckedGetServiceInfo(applicationName, serviceName);

                // The events adapter will include only the services which implement ITisSupportEvents interface. 
                TisServiceEventsAdapterBuilder.AddService(
                    this,
                    applicationName,
                    serviceInstance,
                    serviceInfo,
                    serviceInstanceName);
            }

            return isServiceInstalled;
        }

        public virtual object GetServiceImpl(
            string sHostName,
            string sAppName,
            string sServiceName,
            string sServiceInstanceName)
        {
            if (!StringUtil.IsStringInitialized(sHostName))
            {
                throw new TisException("Empty node name specified");
            }

            if (!StringUtil.IsStringInitialized(sAppName))
            {
                throw new TisException("Empty application name specified");
            }

            if (!StringUtil.IsStringInitialized(sServiceName))
            {
                throw new TisException("Empty service name specified");
            }

            // Validate the caller can obtain service
            // Depends on ServicesHost status and on caller location
            ValidateCanGetService(sAppName, sServiceName);

            if (IsThisHost(sHostName))
            {
                var oService = GetLocalService(
                    sAppName,
                    sServiceName,
                    sServiceInstanceName);

                return oService;
            }
            else
            {
                throw new TisException("Cannot obtain service [{0}/{1}] in application [{2}] from remote host [{3}]",
                    sServiceName, sServiceInstanceName, sAppName, sHostName);
            }
        }

        public virtual void Dispose(bool bDisposing)
        {
            if (bDisposing && !m_bDisposed)
            {
                if (m_oServiceProvidersCache != null)
                {
                    m_oServiceProvidersCache.OnLifetimeManagerActivate -=
                        new TisServiceProviderCache.LifetimeManagerEvent(OnLifetimeManagerActivate);

                    m_oServiceProvidersCache.OnLifetimeManagerDeactivate -=
                        new TisServiceProviderCache.LifetimeManagerEvent(OnLifetimeManagerDeactivate);

                    bool canStopServices;

                    using (RWLockReadSession lockSession = new RWLockReadSession(m_hostStatusLocker))
                    {
                        canStopServices = m_enStatus == HostStatus.Active || m_enStatus == HostStatus.Initializing;
                    }

                    if (canStopServices)
                    {
                        StopApplicationServices(
                            ALL_APPS	// All applications
                            );
                    }

                    m_oServiceProvidersCache.Dispose();
                }

                SetStatus(HostStatus.Inactive);

                if (bDisposing)
                {
                    GC.SuppressFinalize(this);
                }
            }

            m_bDisposed = true;
        }

        private void ValidateCanGetService(
            string applicationName, 
            string serviceName)
        {
            bool canGetService = Status == HostStatus.Active ||
                                 (Status == HostStatus.Initializing && IsThisHost(GetCallingServicesHost()));

            if (!canGetService)
            {
                ExceptionUtil.RaiseInvalidOperationException(
                    MethodInfo.GetCurrentMethod(),
                    "Cannot accept calls in {0} state",
                    Status);
            }

            ITisServiceInfo serviceInfo =
                CheckedGetServiceInfo(applicationName, serviceName);

            canGetService = !IsRestrictedMode ||
                            serviceInfo.UsingMode == ServicesUsingMode.Free;

            if (!canGetService)
            {
                ExceptionUtil.RaiseInvalidOperationException(
                    MethodInfo.GetCurrentMethod(),
                    "Cannot get service {0} in restricted mode",
                    serviceName);
            }
        }

        private string GetCallingServicesHost()
        {
            // TODO: Get real caller ServicesHost.Name
            return Name;
        }

        protected void SetStatus(HostStatus enStatus)
        {
            using (RWLockWriteSession lockSession = new RWLockWriteSession(m_hostStatusLocker)) 
            {
                m_enStatus = enStatus;
            }
        }

        private HostStatus Status
        {
            get 
            {
                using (RWLockReadSession lockSession = new RWLockReadSession(m_hostStatusLocker))
                {
                    return m_enStatus;
                }
            }
        }

        private void CopyServices(
            ITisServiceRegistry oSrc,
            ITisServiceRegistry oDst)
        {
            oDst.ServicesSchema = oSrc.ServicesSchema;

            foreach (ITisServiceInfo oServiceInfo in oSrc.InstalledServices)
            {
                if (!oServiceInfo.FromSchema)
                {
                    oDst.InstallService(
                        oServiceInfo.ServiceName,
                        oServiceInfo.ServiceCreatorType,
                        oServiceInfo.ServiceImplType,
                        oServiceInfo.RequiredRoles,
                        oServiceInfo.UsingMode == ServicesUsingMode.Free);
                }
            }
        }

        private string ServiceNameFromServiceType(
            Type oServiceType)
        {
            return TisServicesUtil.ServiceNameFromServiceType(oServiceType);
        }

        private string SelectServicesHost(
            string sAppName,
            string sServiceName)
        {
            ITisServiceInfo oServiceInfo = CheckedGetServiceInfo(
                sAppName,
                sServiceName);

            if (oServiceInfo.RequiredRoles.Length == 0)
            {
                return this.Name;
            }

            if (Array.IndexOf(oServiceInfo.RequiredRoles, TisServicesConst.SERVER_ROLE) >= 0)
            {
                return BasicConfiguration.ServerMachineName; 
            }

            throw new TisException("Not supported service required roles.");
        }

        private void InitServiceProvidersCache()
        {
            // Create service provider factory
            TisServiceProviderFactory oFactory =
                new TisServiceProviderFactory(this);

            // Create service provider cache
            m_oServiceProvidersCache =
                new TisServiceProviderCache(oFactory);

            // Subscribe ServiceProviderCache events
            m_oServiceProvidersCache.OnLifetimeManagerActivate +=
                new TisServiceProviderCache.LifetimeManagerEvent(OnLifetimeManagerActivate);

            m_oServiceProvidersCache.OnLifetimeManagerDeactivate +=
                new TisServiceProviderCache.LifetimeManagerEvent(OnLifetimeManagerDeactivate);
        }

        #region Service Lifetime Events Handling

        private void OnLifetimeManagerActivate(
            string sAppName,
            ITisServiceLifetimeManager oLifetimeManager)
        {
            oLifetimeManager.OnPreServiceActivate += new TisServiceLifetimeEvent(LifetimeManager_OnPreServiceActivate);
            oLifetimeManager.OnPostServiceActivate += new TisServiceLifetimeEventEx(LifetimeManager_OnPostServiceActivate);
            oLifetimeManager.OnPreServiceDeactivate += new TisServiceLifetimeEventEx(LifetimeManager_OnPreServiceDeactivate);
            oLifetimeManager.OnPostServiceDeactivate += new TisServiceLifetimeEvent(LifetimeManager_OnPostServiceDeactivate);
        }

        private void OnLifetimeManagerDeactivate(
            string sAppName,
            ITisServiceLifetimeManager oLifetimeManager)
        {
            oLifetimeManager.OnPreServiceActivate -= new TisServiceLifetimeEvent(LifetimeManager_OnPreServiceActivate);
            oLifetimeManager.OnPostServiceActivate -= new TisServiceLifetimeEventEx(LifetimeManager_OnPostServiceActivate);
            oLifetimeManager.OnPreServiceDeactivate -= new TisServiceLifetimeEventEx(LifetimeManager_OnPreServiceDeactivate);
            oLifetimeManager.OnPostServiceDeactivate -= new TisServiceLifetimeEvent(LifetimeManager_OnPostServiceDeactivate);
        }

        private void LifetimeManager_OnPreServiceActivate(
            string sAppName,
            TisServiceKey oServiceKey)
        {
            if (OnPreServiceActivate != null)
            {
                OnPreServiceActivate(sAppName, oServiceKey);
            }
        }

        private void LifetimeManager_OnPostServiceActivate(
            string sAppName,
            TisServiceKey oServiceKey,
            object oService)
        {
            ITisServiceInfo serviceInfo = oServiceKey.ServiceInfo;

            if (serviceInfo.SupportedEvents.Count > 0)
            {
                TisServiceEventsAdapterBuilder.AddService(
                    this,
                    sAppName,
                    oService,
                    serviceInfo,
                    oServiceKey.InstanceName);
            }

            if (OnPostServiceActivate != null)
            {
                OnPostServiceActivate(sAppName, oServiceKey);
            }
        }

        private void LifetimeManager_OnPreServiceDeactivate(
            string sAppName,
            TisServiceKey oServiceKey,
            object oService)
        {
            if (OnPreServiceDeactivate != null)
            {
                OnPreServiceDeactivate(sAppName, oServiceKey);
            }

            ReleaseService(sAppName, oServiceKey.ServiceName, oServiceKey.InstanceName);
        }

        private void LifetimeManager_OnPostServiceDeactivate(
            string sAppName,
            TisServiceKey oServiceKey)
        {
            if (OnPostServiceDeactivate != null)
            {
                OnPostServiceDeactivate(sAppName, oServiceKey);
            }
        }

        #endregion

        private object GetLocalService(
            string sAppName,
            string sServiceName,
            string sServiceInstanceName)
        {
            // Validate sAppName
            if (!StringUtil.IsStringInitialized(sAppName))
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sAppName",
                    "sAppName parameter must be initialized",
                    MethodInfo.GetCurrentMethod());
            }

            // Validate sServiceName
            if (!StringUtil.IsStringInitialized(sServiceName))
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sServiceName",
                    "sServiceName parameter must be initialized",
                    MethodInfo.GetCurrentMethod());
            }

            // Validate sServiceInstanceName
            if (!StringUtil.IsStringInitialized(sServiceInstanceName))
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sServiceInstanceName",
                    "sServiceInstanceName parameter must be initialized",
                    MethodInfo.GetCurrentMethod());
            }

            ITisServiceProvider oServiceProvider = null;

            // Get service provider for the application
            oServiceProvider = m_oServiceProvidersCache.GetServiceProvider(sAppName);

            ITisServiceInfo oServiceInfo =
                CheckedGetServiceInfo(sAppName, sServiceName);

            // Create service key
            TisServiceKey oKey = new TisServiceKey(
                    sServiceName,
                    sServiceInstanceName,
                    oServiceInfo);

            object oService = oServiceProvider.GetService(
                oKey);

            return oService;
        }

        private bool IsServiceInstantiated(
            string sAppName,
            string sServiceName,
            string sServiceInstanceName)
        {
            // Validate sAppName
            if (!StringUtil.IsStringInitialized(sAppName))
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sAppName",
                    "sAppName parameter must be initialized",
                    MethodInfo.GetCurrentMethod());
            }

            // Validate sServiceName
            if (!StringUtil.IsStringInitialized(sServiceName))
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sServiceName",
                    "sServiceName parameter must be initialized",
                    MethodInfo.GetCurrentMethod());
            }

            // Validate sServiceInstanceName
            if (!StringUtil.IsStringInitialized(sServiceInstanceName))
            {
                ExceptionUtil.RaiseArgumentNullException(
                    "sServiceInstanceName",
                    "sServiceInstanceName parameter must be initialized",
                    MethodInfo.GetCurrentMethod());
            }

            ITisServiceProvider oServiceProvider = null;

            // Get service provider for the application
            oServiceProvider = m_oServiceProvidersCache.GetServiceProvider(sAppName);

            // Create service key
            TisServiceKey oKey = new TisServiceKey(
                sServiceName,
                sServiceInstanceName);

            return oServiceProvider.IsInstantiated(oKey);
        }

        protected bool IsThisHost(string sHostName)
        {
            return TisServicesUtil.ServicesHostNamesEqual(
                sHostName,
                this.Name);
        }

        private bool IsActive
        {
            get
            {
                using (RWLockReadSession lockSession = new RWLockReadSession(m_hostStatusLocker))
                {
                    return m_enStatus == HostStatus.Active;
                }
            }
        }
    }

    #endregion

    #region TisLocalServicesHost

    public class TisLocalServicesHost : TisServicesHost
    {
        public TisLocalServicesHost()
        {
            BasicConfiguration basicConfiguration = new BasicConfiguration();
            this.Initialize(basicConfiguration);
        }
    }

    #endregion
}
