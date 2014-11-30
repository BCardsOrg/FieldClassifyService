using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.IO;
using System.ServiceModel.Description;
using TiS.Core.TisCommon.Network;
using TiS.Core.TisCommon.Configuration;
using System.Collections.Concurrent;
using System.Security.Authentication;

namespace TiS.Core.TisCommon.Services.Web
{
    #region TisWebServicesHost

    public class TisWebServiceClientHost : TisServicesHost
    {
        private ConcurrentDictionary<TisWebServiceClientInfo, TisWebServiceClient> m_cachedWebServiceClients = new ConcurrentDictionary<TisWebServiceClientInfo, TisWebServiceClient>();

        private byte[] m_lastStationInstancePermission = null;
        public string ApplicationName { get; set; }

        private BasicConfiguration m_basicConfiguration;
        private ClientChannelFactoryConfiguration m_clientChannelFactoryConfiguration;

        private string m_communicationProtocol;

        public TisWebServiceClientHost(
            string userName = null,
            string password = null,
            string communicationProtocol = null)
        {
            UserName = String.Empty;
            m_basicConfiguration = new BasicConfiguration();
            m_clientChannelFactoryConfiguration = new ClientChannelFactoryConfiguration(m_basicConfiguration);
            m_communicationProtocol = StringUtil.IsStringInitialized(communicationProtocol) ? communicationProtocol : m_basicConfiguration.CommunicationProtocol;

            if (StringUtil.IsStringInitialized(userName) || StringUtil.IsStringInitialized(password))
            {
                m_clientChannelFactoryConfiguration.SetClientCredentials(userName, password);
            }
        }

        public void ForcePermission(byte[] instancePermission)
        {
            m_lastStationInstancePermission = instancePermission;
        }

        public void SetInstancePermission(string appName, string stationName, string userName, byte[] instancePermission)
        {
            ApplicationName = appName;
            UserName = userName;

            if ((m_lastStationInstancePermission == null) || !string.IsNullOrEmpty(stationName))
            {
                m_lastStationInstancePermission = instancePermission;
            }
        }

        public override void StopApplicationServices(string sAppName)
        {
            lock (this)
            {
                base.StopApplicationServices(sAppName);

                List<KeyValuePair<TisWebServiceClientInfo, TisWebServiceClient>> webServiceClients = GetWebServiceClientsByApplication(sAppName);

                if (webServiceClients != null)
                {
                    foreach (KeyValuePair<TisWebServiceClientInfo, TisWebServiceClient> kvp in webServiceClients)
                    {
                        kvp.Value.Dispose();
                        TisWebServiceClient webServiceClient;
                        m_cachedWebServiceClients.TryRemove(kvp.Key, out webServiceClient);
                    }
                }
            }
        }

        public override object GetServiceImpl(string sHostName, string sAppName, string sServiceName, string sServiceInstanceName)
        {
            ITisServiceInfo serviceInfo = CheckedGetServiceInfo(
                sAppName,
                sServiceName);

            if (!IsWebServiceSingleton(serviceInfo) && IsThisHost(sHostName))
            {
                return base.GetServiceImpl(sHostName, sAppName, sServiceName, sServiceInstanceName);
            }

            if (!StringUtil.IsStringInitialized(serviceInfo.ServiceImplType))
            {
                throw new TisException("Service {0} has no implementation type", serviceInfo.ServiceName);
            }

            return CreateWebServiceClient(
                sHostName,
                sAppName,
                (ITisWebServiceInfo)serviceInfo,
                sServiceInstanceName);
        }

        private bool IsWebServiceSingleton(ITisServiceInfo serviceInfo)
        {
            return (serviceInfo is ITisWebServiceInfo) &&
                    ((ITisWebServiceInfo)serviceInfo).WebServiceIsSingleton;
        }

        public void SetWebServiceCallClientContextData(
            object webServiceClientChannel,
            string stationId)
        {
            lock (this)
            {
                TisWebServiceContextData header = (TisWebServiceContextData)GetWebServiceClientHeader(webServiceClientChannel);

                if (header != null)
                {
                    header.StationId = stationId;
                }
            }
        }

        private object CreateWebServiceClient(
            string sHostName,
            string sAppName,
            ITisWebServiceInfo oServiceInfo,
            string serviceInstanceName)
        {
            try
            {
                return GetWebServiceClientChannel(sHostName, sAppName, oServiceInfo, serviceInstanceName);
            }
            catch
            {
                // Try with IP address (probably DNS is unavailable)
                string hostIPAddress = NetworkUtil.GetIPAddress(sHostName);

                Log.WriteInfo("Failed to get service via host name : [{0}]. Trying via IP address : [{1}]", sHostName, hostIPAddress);

                return GetWebServiceClientChannel(hostIPAddress, sAppName, oServiceInfo, serviceInstanceName);
            }
        }

        private object GetWebServiceClientChannel(
            string sHostName,
            string sAppName,
            ITisWebServiceInfo oServiceInfo,
            string serviceInstanceName)
        {
            lock (this)
            {
                string webServiceUri = CreateWebServiceURI(
                    sHostName,
                    sAppName,
                    oServiceInfo.ServiceName);

                TisWebServiceClient webServiceClient =
                    GetWebServiceClientByUri(sAppName, webServiceUri);

                TisWebServiceContextData webServiceContextData = new TisWebServiceContextData()
                {
                    Name = oServiceInfo.ServiceName,
                    ApplicationName = sAppName,
                    InstanceName = serviceInstanceName,
                    CreatorTypeName = oServiceInfo.ServiceCreatorType,
                    NodeName = Name,
                    Version = ModuleVersion.PlatformVersion.ToString(),
                    InstancePermission = GetInstancePermission(oServiceInfo, sAppName, StationName),
                    StationName = StationName,
                    StationId = StationId,
                    UserName = UserName
                };

                if (webServiceClient == null)
                {
                    m_clientChannelFactoryConfiguration.EndPointConfigurationName = GetWebServiceConfigurationName(oServiceInfo);

                    webServiceClient = new TisWebServiceClient(
                        oServiceInfo.WebServiceContractType,
                        m_clientChannelFactoryConfiguration,
                        webServiceContextData,
                        webServiceUri);

                    m_cachedWebServiceClients.TryAdd(new TisWebServiceClientInfo(sAppName, webServiceUri), webServiceClient);
                }

                webServiceClient.Header = webServiceContextData;

                return webServiceClient.ClientChannel;
            }
        }

        private string GetWebServiceConfigurationName(ITisWebServiceInfo oServiceInfo)
        {
            string serviceConfigurationName = oServiceInfo.WebServiceConfigurationName;

            if (!StringUtil.IsStringInitialized(serviceConfigurationName))
            {
                serviceConfigurationName = oServiceInfo.ServiceName + "_" + m_communicationProtocol;
            }

            return serviceConfigurationName;
        }

        private TisWebServiceClient GetWebServiceClientByUri(
            string applicationName,
            string webServiceUri)
        {
            TisWebServiceClient webServiceClient = null;

            var webServiceClientsInfo =
                (from webServiceClientInfo in m_cachedWebServiceClients.Keys
                 where StringUtil.CompareIgnoreCase(webServiceClientInfo.ApplicationName, applicationName) &&
                       StringUtil.CompareIgnoreCase(webServiceClientInfo.WebServiceUri, webServiceUri)
                 select webServiceClientInfo).FirstOrDefault();



            if (webServiceClientsInfo != null)
            {
                m_cachedWebServiceClients.TryGetValue(webServiceClientsInfo, out webServiceClient);
            }


            return webServiceClient;
        }

        private List<KeyValuePair<TisWebServiceClientInfo, TisWebServiceClient>> GetWebServiceClientsByApplication(string applicationName)
        {
            if (!StringUtil.IsStringInitialized(applicationName))
            {
                return m_cachedWebServiceClients.ToList();
            }

            IEnumerable<KeyValuePair<TisWebServiceClientInfo, TisWebServiceClient>> webServiceClientsInfo =
                from webServiceClient in m_cachedWebServiceClients
                where StringUtil.CompareIgnoreCase(webServiceClient.Key.ApplicationName, applicationName)
                select webServiceClient;

            bool webServiceClientsExist = webServiceClientsInfo.Count() > 0;

            if (webServiceClientsExist)
            {
                return m_cachedWebServiceClients.Intersect(webServiceClientsInfo).ToList();
            }

            return null;
        }

        private object GetWebServiceClientHeader(object webServiceClientChannel)
        {
            IEnumerable<TisWebServiceClient> webServiceClients =
                from webServiceClient in m_cachedWebServiceClients.Values
                where webServiceClient.ClientChannel.Equals(webServiceClientChannel)
                select webServiceClient;

            bool webServiceClientExist = webServiceClients.Count() > 0;

            if (webServiceClientExist)
            {
                return webServiceClients.ToArray()[0].Header;
            }

            return null;
        }

        private string CreateWebServiceURI(
            string hostId,
            string sAppName,
            string serviceName)
        {
            string sUri = String.Format(@"{0}://{1}/{2}/{3}{4}",
                TisWebServiceExtensions.StringToScheme(null, m_communicationProtocol),
                hostId,
                WebConfigConstants.EFLOW_WEB_APP_NAME,
                serviceName, // service relative WEB address
                WebConfigConstants.EFLOW_WEB_SERVICE_EXT);

            return sUri;
        }

        public byte[] InstancePermission
        {
            get
            {
                return m_lastStationInstancePermission;
            }
        }

        private byte[] GetInstancePermission(
            ITisWebServiceInfo oServiceInfo,
            string appName,
            string stationName)
        {
            if (m_lastStationInstancePermission == null && !IsClaimsService(oServiceInfo))
            {
                throw new AuthenticationException(string.Format("Missing permission for application {0} / station {1}", appName, stationName));
            }

            return m_lastStationInstancePermission;
        }

        private bool IsClaimsService(ITisWebServiceInfo oServiceInfo)
        {
            return StringUtil.CompareIgnoreCase(oServiceInfo.ServiceName, TisServicesConst.CLAIMS_SERVICE_WINDOWS) ||
                   StringUtil.CompareIgnoreCase(oServiceInfo.ServiceName, TisServicesConst.CLAIMS_SERVICE_USER);
        }

        private string GetPermissionName(string appName, string stationName)
        {
            if (appName == TisServicesConst.SystemApplication)
                return TisServicesConst.SystemApplication;
            else
                return appName + @"\" + stationName;
        }

        #region TisWebServiceClientInfo

        private class TisWebServiceClientInfo
        {
            public TisWebServiceClientInfo(string applicationName, string webServiceUri)
            {
                ApplicationName = applicationName;
                WebServiceUri = webServiceUri;
            }

            public string ApplicationName { get; private set; }
            public string WebServiceUri { get; private set; }
        }

        #endregion

    }

    #endregion

    #region ClientChannelFactoryConfiguration

    public class ClientChannelFactoryConfiguration
    {
        public ClientChannelFactoryConfiguration(BasicConfiguration basicConfiguration)
        {
            IsCustomConfig = ProcessConfiguration.IsCustomConfig;
            ServiceModelSection = ProcessConfiguration.ClientServiceModelSection;

            if (IsCustomConfig)
            {
                if (ServiceModelSection == null)
                {
                    Log.WriteError("<system.serviceModel> section does not exist in custom configuration file [{0}].", ProcessConfiguration.ProcessConfigFile);
                }
                else
                {
                    Log.WriteInfo("Using [{0}] as a custom configuration file.", ProcessConfiguration.ProcessConfigFile);
                }
            }
        }

        public string EndPointConfigurationName { get; set; }
        public bool IsCustomConfig { get; private set; }

        public ServiceModelSectionGroup ServiceModelSection { get; set; }
        public ClientCredentials Credentials { get; set; }

        public void SetClientCredentials(
            string userName,
            string password)
        {
            Credentials = new ClientCredentials();

            Credentials.UserName.UserName = userName;
            Credentials.UserName.Password = password;
        }
    }



    //public class ClientChannelFactoryConfiguration
    //{
    //    public ClientChannelFactoryConfiguration(BasicConfiguration basicConfiguration)
    //    {
    //        UseArbitraryConfiguration = basicConfiguration.UseConfigSource;
    //        ArbitraryConfiguration = basicConfiguration.eFlowConfigSource;

    //        if (!Path.IsPathRooted(ArbitraryConfiguration))
    //        {
    //            ArbitraryConfiguration = Path.Combine(basicConfiguration.eFlowBinPath, ArbitraryConfiguration);
    //        }

    //        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();

    //        if (IsDevelopementProcess(currentProcess.ProcessName))
    //        {
    //            IsDefaultConfiguration = false;
    //        }
    //        else
    //        {
    //            var processName =
    //                currentProcess.ProcessName == TisServicesConst.IIS_PROCESS_NAME ? TisServicesConst.IIS_PROCESS_FILE_NAME : currentProcess.MainModule.FileName;

    //            DefaultConfigFile = processName + TisServicesConst.CONFIG_FILE_EXT;

    //            IsDefaultConfiguration = File.Exists(DefaultConfigFile);
    //        }

    //        if (IsDefaultConfiguration)
    //        {
    //            Log.WriteInfo("Using default configuration file [{0}].", DefaultConfigFile);
    //        }
    //        else
    //        {
    //            Log.WriteWarning("Default configuration file [{0}] does not exist.", DefaultConfigFile);

    //            IsDefaultConfiguration =
    //                !UseArbitraryConfiguration ||
    //                !File.Exists(ArbitraryConfiguration);

    //            if (!IsDefaultConfiguration)
    //            {
    //                ExeConfigurationFileMap map = new ExeConfigurationFileMap();

    //                map.ExeConfigFilename = ArbitraryConfiguration;

    //                System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

    //                ServiceModelSection = ServiceModelSectionGroup.GetSectionGroup(configuration);

    //                if (ServiceModelSection == null)
    //                {
    //                    Log.WriteWarning("<system.serviceModel> section does not exist in custom configuration file [{0}].", ArbitraryConfiguration);
    //                }
    //                else
    //                {
    //                    Log.WriteInfo("Using [{0}] as a custom configuration file.", ArbitraryConfiguration);
    //                }
    //            }
    //        }
    //    }

    //    private bool IsDevelopementProcess(string processName)
    //    {
    //        bool isDev = (StringUtil.CompareIgnoreCase(processName, TisServicesConst.VS_PROCESS_NAME))
    //            || (StringUtil.ArrayContainsIgnoreCase(TisServicesConst.VS_TEST_PROCESS_NAMES, processName));

    //        return isDev;
    //    }

    //    public bool IsDefaultConfiguration { get; set; }
    //    public bool UseArbitraryConfiguration { get; set; }
    //    public string EndPointConfigurationName { get; set; }
    //    public string ArbitraryConfiguration { get; set; }
    //    public string DefaultConfigFile { get; set; }

    //    public ServiceModelSectionGroup ServiceModelSection { get; set; }
    //    public ClientCredentials Credentials { get; set; }

    //    public void SetClientCredentials(
    //        string userName,
    //        string password)
    //    {
    //        Credentials = new ClientCredentials();

    //        Credentials.UserName.UserName = userName;
    //        Credentials.UserName.Password = password;
    //    }
    //}

    #endregion
}
