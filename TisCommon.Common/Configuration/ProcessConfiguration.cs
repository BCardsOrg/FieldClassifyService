using System;
using System.IO;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Services;
using System.ServiceModel.Configuration;

namespace TiS.Core.TisCommon.Configuration
{
    /*
     
    1. TisConfiguration.xml and TisLogger.xml should be renamed to *.config and placed 
       - eFlow_5, TiSDefaultSTS : alongside with web.config;
       - WebCompletion : in ConfigSources folder. 
       
    2.  Web.config should include :
       - eFlow_5 :
            <appSettings>
                <add key="BasicConfigurationFile" value="TISconfiguration.config"/>
                <add key="LoggerFile" value="TISLogger.config"/>
            </appSettings>
       - TiSDefaultSTS :
            <appSettings>
                <add key="BasicConfigurationFile" value="TISconfiguration.config"/>
                <add key="LoggerFile" value="TISLogger.config"/>
            </appSettings>
       - WebCompletion :  
            <appSettings>
                <add key="BasicConfigurationFile" value="Bin\ConfigSources\TISconfiguration.config"/>
                <add key="LoggerFile" value="Bin\ConfigSources\TISLogger.config"/>
            </appSettings>
     
    3. Web.config of WebCompletion should include :
          <system.serviceModel>
              <bindings configSource="Bin\ConfigSources\BindingSection.config"></bindings>
              <client configSource="Bin\ConfigSources\ClientSection.config"></client>
              <behaviors configSource="Bin\ConfigSources\BehaviorSection.config"></behaviors>
          </system.serviceModel>  
     
    4. All client <application>|<default>.exe.config should include :
        <appSettings>
            <add key="BasicConfigurationFile" value="ConfigSources\TISconfiguration.config"/>
            <add key="LoggerFile" value="ConfigSources\TISLogger.config"/>
        </appSettings>

    */

    [ComVisible(false)]
    public class ProcessConfiguration
    {
        private const string APPSETTING_KEY_BASIC            = "BasicConfigurationFile";
        private const string APPSETTING_KEY_LOGGER           = "LoggerFile";
        private const string DEFAULT_CONFIG_FILE             = "Default.exe.config";

        private const string CLIENT_DEFAULT_FOLDER           = "ConfigSources";
        private const string SERVER_DEFAULT_FOLDER           = @"Bin\ConfigSources";

        private const string BASIC_CONFIG_DEFAULT_FILE_NAME  = "TISConfiguration.config";
        private const string LOGGER_CONFIG_DEFAULT_FILE_NAME = "TISLogger.config";

        public static bool   IsCustomConfig    { get; private set; }
        public static string ProcessConfigFile { get; private set; }
        public static string BasicConfigFile   { get; private set; }
        public static string LoggerConfigFile  { get; private set; }
        public static ServiceModelSectionGroup ClientServiceModelSection { get; private set; }

        static ProcessConfiguration()
        {
            try
            {
                System.Configuration.Configuration config;
                string folderName;

                var exeName = PathUtil.GetExeName();

                var isWebApplication = StringUtil.CompareIgnoreCase(
                    Path.GetFileNameWithoutExtension(exeName), TisServicesConst.IIS_PROCESS_NAME);

                bool isClientConfiguration = true;

                if (isWebApplication)
                {
                    folderName = HttpRuntime.AppDomainAppPath;

                    config = WebConfigurationManager.OpenWebConfiguration(
                        System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath,
                        System.Web.Hosting.HostingEnvironment.SiteName);

                    ProcessConfigFile = config.FilePath;

                    isClientConfiguration = System.Web.Hosting.HostingEnvironment.ApplicationID.EndsWith(WebConfigConstants.EFLOW_WEB_APP_NAME) == false;

                    if (isClientConfiguration)
                    {
                        // WebCompletion eFlow server
                        ClientServiceModelSection = ServiceModelSectionGroup.GetSectionGroup(config);
                    }
                }
                else
                {
                    if (IsDevelopementProcess(Path.GetFileNameWithoutExtension(exeName)))
                    {
                        // devenv (Visual Studio)
                        folderName = Path.Combine(RegistryUtil.eFlowPath, @"Bin");
                        ProcessConfigFile = String.Empty;
                    }
                    else
                    {
                        // eFlow client
                        folderName = PathUtil.GetExePath();
                        ProcessConfigFile = Path.Combine(folderName, exeName + TisServicesConst.CONFIG_FILE_EXT);
                    }

                    if (!File.Exists(ProcessConfigFile))
                    {
                        IsCustomConfig = true;
                        ProcessConfigFile = Path.Combine(folderName, DEFAULT_CONFIG_FILE);
                    }

                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

                    fileMap.ExeConfigFilename = ProcessConfigFile;

                    config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                    ClientServiceModelSection = ServiceModelSectionGroup.GetSectionGroup(config);
                }

                var basicConfigElement = config.AppSettings.Settings[APPSETTING_KEY_BASIC];
                var loggerConfigElement = config.AppSettings.Settings[APPSETTING_KEY_LOGGER];

                var basicConfigFilePart = basicConfigElement != null ? basicConfigElement.Value :
                    isClientConfiguration ? Path.Combine(CLIENT_DEFAULT_FOLDER, BASIC_CONFIG_DEFAULT_FILE_NAME) : Path.Combine(SERVER_DEFAULT_FOLDER, BASIC_CONFIG_DEFAULT_FILE_NAME);

                var loggerConfigFilePart = loggerConfigElement != null ? loggerConfigElement.Value :
                    isClientConfiguration ? Path.Combine(CLIENT_DEFAULT_FOLDER, LOGGER_CONFIG_DEFAULT_FILE_NAME) : Path.Combine(SERVER_DEFAULT_FOLDER, LOGGER_CONFIG_DEFAULT_FILE_NAME);

                BasicConfigFile  = Path.Combine(folderName, basicConfigFilePart);
                LoggerConfigFile = Path.Combine(folderName, loggerConfigFilePart);

                if (isWebApplication)
                {
                    Log.WriteInfo("HostingEnvironment : ApplicationID : [{0}], ApplicationPhysicalPath : [{1}], ApplicationVirtualPath : [{2}].",
                        System.Web.Hosting.HostingEnvironment.ApplicationID,
                        System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath,
                        System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                }

                Log.WriteInfo("In use : Process configuration file : [{0}], Basic configuration file : [{1}], Logger configuration file : [{2}].", 
                    ProcessConfigFile, BasicConfigFile, LoggerConfigFile);
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);
            }
        }
        private static bool IsDevelopementProcess(string processName)
        {
            bool isDev = (StringUtil.CompareIgnoreCase(processName, TisServicesConst.VS_PROCESS_NAME))
                || (StringUtil.ArrayContainsIgnoreCase(TisServicesConst.VS_TEST_PROCESS_NAMES, processName));

            return isDev;
        }
    }
}
