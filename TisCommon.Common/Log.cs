using System;
using System.Reflection;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon
{
    public enum TIS_SEVERITY
    {
        TIS_DEBUG = 2,
        TIS_DETAILED_DEBUG = 1,
        TIS_ERROR = 5,
        TIS_FATAL_ERROR = 6,
        TIS_INFORMATORY = 3,
        TIS_MAX_SEVERITY = 7,
        TIS_MIN_SEVERITY = 0,
        TIS_WARNING = 4
    }

    /// <summary>
    /// TIS logger wrapper class.
    /// </summary>
    public class Log
    {
        private TiS.Logger.Log m_logInstance;

        public enum Severity
        {
            TIS_MIN_SEVERITY = 0,
            DETAILED_DEBUG,
            DEBUG,
            INFO,
            WARNING,
            ERROR,
            FATAL_ERROR,
            TIS_MAX_SEVERITY = 7
        }

        static Log()
        {
            TiS.Logger.Log.OnConfigFileLocation += new Logger.OnConfigFileLocationDelegate(Log_OnConfigFileLocation);
        }

        public static Severity MinSeverity
        {
            get
            {
                return (Severity)TiS.Logger.Log.MinSeverity;
            }
            set { TiS.Logger.Log.MinSeverity = (TiS.Logger.Log.Severity)value; }
        }

        public bool Initialize()
        {
            return LogInstance.Initialize();
        }

        public bool setAppender(TiS.Logger.TISAppender appender)
        {
            return LogInstance.setAppender(appender);
        }

        public static bool Initialize(string configFile, string configLogger)
        {
            return TiS.Logger.Log.Initialize(configFile, configLogger);
        }

        public static void SetLogPath(string logDirectory)
        {
            TiS.Logger.Log.SetLogPath(logDirectory);
        }

        public static string GetLogPath()
        {
             TisCommon.Configuration.GlobalConfigurationService globalSrv = new TisCommon.Configuration.GlobalConfigurationService();
             string temp = globalSrv.GetConfigFilesPath();
             if (temp.Length <= 0)
                 return string.Empty;

             return temp.Replace("Configuration", "Log");
        }



        public static void SetMinSeverity(Severity enMinSeverity)
        {
            TiS.Logger.Log.SetMinSeverity((TiS.Logger.Log.Severity)enMinSeverity);
        }

        #region OldCode

        public void RequestMessageLog(string Message, string UnitName, TiS.Logger.Log.Severity Severity)
        {
            LogInstance.RequestMessageLog(Message, UnitName, Severity);
        }

        public void RequestMessageLog(string Message, string UnitName, TiS.Logger.Log.Severity Severity, Exception exception)
        {
            LogInstance.RequestMessageLog(Message, UnitName, Severity, exception);
        }

        public static void SetLogFileLocation(string logFilePath)
        {
            TiS.Logger.Log.SetLogFileLocation(logFilePath);
        }

        public static void Write(Severity enSeverity, params object[] Params)
        {
            TiS.Logger.Log.Write((TiS.Logger.Log.Severity)enSeverity, Params);
        }

        public static void Write(Severity enSeverity, MethodBase oMethod, string sFormat, params object[] Params)
        {
            TiS.Logger.Log.Write((TiS.Logger.Log.Severity)enSeverity, oMethod, sFormat, Params);
        }

        public static void WriteDebug(string sFormat, params object[] Params)
        {
            TiS.Logger.Log.WriteDebug(sFormat, Params);
        }

        public static void WriteDetailedDebug(string sFormat, params object[] Params)
        {
            TiS.Logger.Log.WriteDetailedDebug(sFormat, Params);
        }

        public static void WriteInfo(string sFormat, params object[] Params)
        {
            TiS.Logger.Log.WriteInfo(sFormat, Params);
        }

        public static void WriteWarning(string sFormat, params object[] Params)
        {
            TiS.Logger.Log.WriteWarning(sFormat, Params);
        }

        public static void WriteError(string sFormat, params object[] Params)
        {
            TiS.Logger.Log.WriteError(sFormat, Params);
        }

        public static void WriteFatalError(string sFormat, params object[] Params)
        {
            TiS.Logger.Log.WriteFatalError(sFormat, Params);
        }

        public static void WriteException(Exception E)
        {
            TiS.Logger.Log.WriteException(E);
        }

        public static void WriteException(Severity enSeverity, Exception E, params object[] Params)
        {
            TiS.Logger.Log.WriteException((TiS.Logger.Log.Severity)enSeverity, E, Params);
        }

        #endregion

        private TiS.Logger.Log LogInstance
        {
            get
            {
                if (m_logInstance == null)
                {
                    m_logInstance = new Logger.Log();
                }

                return m_logInstance;
            }
        }

        private static string Log_OnConfigFileLocation(object sender, EventArgs args)
        {
            return ProcessConfiguration.LoggerConfigFile;
        }
    }
}
