using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Repository;
using TISAppenderLog4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace TiS.Logger
{
    public delegate string OnConfigFileLocationDelegate(object sender, EventArgs args);

    public enum TISAppender
    {
        TISLogger
    }

    [Guid("13F5604E-4A87-48EA-BA3A-E6AA64FC32BA")]
    [ComVisible(true)]
    public interface ITisLog
    {
        bool Initialize();
        bool setAppender(TISAppender appender);

        void RequestMessageLog(string Message, string UnitName, Log.Severity Severity);
        void RequestMessageLog(string Message, string UnitName, Log.Severity Severity, Exception exception);
    }


    /// <summary>
    /// TIS logger class.
    /// this class work through log4net.
    /// To use: 1.- Create a instance of Log.
    ///         2.- Initialize([log4net configuration file(usually c:\\TIS_Logger\\logger.config)], [logger mode("TIS_FileAppender", "TIS_RollingFileAppender", "TIS_Consol"...)]);
    ///                                 
    /// </summary>
    public class Log : ITisLog
    {
        private static Mutex TISMutex;
        private const int MUTEX_LOCKING_TIMEOUT = 5000;

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

        public static string EVENT_LOG_NAME          = "TIS_Log";
        public static string EVENT_LOG_SOURCE_SERVER = "TIS_SERVER";
        public static string EVENT_LOG_SOURCE_STS    = "TIS_STS";
        public static string EVENT_LOG_SOURCE_CLIENT = "TIS_CLIENT";

        private static Severity m_enMinSeverity = Severity.WARNING;
        public static Severity MinSeverity
        {
            get
            {
                lock (typeof(Log))
                {
                    return m_enMinSeverity;
                }
            }
            set { m_enMinSeverity = value; }
        }

        private static bool isInitialized = false;
        private static string logPath;
        private static TISLogConfiguration logCfng;
        private static string cfgLogger;
        private static Queue waitingLst = new Queue();

        static Log()
        {
            TISLogConfiguration.OnConfigFileLocation += new OnConfigFileLocationDelegate(LogConfiguration_OnConfigFileLocation);
            cfgLogger = "TIS_Logger";
            logCfng = new TISLogConfiguration();
        }

        public static event OnConfigFileLocationDelegate OnConfigFileLocation;

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        public bool Initialize()
        {

            return Initialize(LogPath, cfgLogger);
        }


        static object m_lock = new object();
        /// <summary>
        /// configFile : Initializes the logger to use a specific configuration file.
        /// configLogger : set log4Net to specific log type , TIS_FileAppender, TIS_RollingFileAppender, TIS_Logger(to use with TIS_Monitor)
        /// </summary>
        public static bool Initialize(string configFile, string configLogger)
        {
            if (isInitialized == true)
            {
                return true;
            }

            lock (m_lock)
            {
                if (isInitialized == true)
                {
                    return true;
                }

                if (configFile != null)
                {
                    if (!File.Exists(configFile))
                        return false;
                }

                if (configFile == null)
                {
                    log4net.Config.XmlConfigurator.Configure();
                }
                else
                {
                    try
                    {
                        if (configLogger.Length > 0)
                            cfgLogger = configLogger;

                        if (!isInitialized)
                        {
                            log4net.Config.XmlConfigurator.Configure(new MemoryStream(File.ReadAllBytes(configFile)));
                            isInitialized = true;
                        }

                    }
                    catch
                    {
                        isInitialized = false;
                    }
                }
            }

            ILog log = LogManager.GetLogger(cfgLogger);

            return isInitialized;
        }

        /// <summary>
        /// Logs an entry to the specified log.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="loggingLevel">The Severity level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">Any exception to be logged.</param>
        /// <exception cref="InvalidLogException">Thrown if <paramref name="logName"/> does not exist.</exception>
        /// 
        public static void Logger(string logName, Severity loggingLevel, string message, Log4NetException excp)
        {
            Initialize(LogPath, logName);

            ILog log = LogManager.GetLogger(logName);
            if (log != null)
            {
                LogBase(log, loggingLevel, message, excp);
                //Flush the message queue...
                try
                {
                    for (int indx = 0; indx < waitingLst.Count; indx++)
                    {
                        logItem qItem = (logItem)waitingLst.Dequeue();
                        LogBase(log, qItem.loggingLevel, qItem.message, qItem.obj as Log4NetException);
                    }
                    waitingLst.Clear();
                }
                catch
                {
                    if (waitingLst != null)
                        waitingLst.Clear();
                }
            }
        }

        public bool setAppender(TISAppender appender)
        {
            return true;
        }

        private static string LogPath
        {
            get
            {
                if (logPath == null)
                {
                    logPath = logCfng.ConfigFileLocation;
                }

                return logPath;
            }
            set
            {
                logPath = value;
            }
        }

        private static string LogConfiguration_OnConfigFileLocation(object sender, EventArgs args)
        {
            if (OnConfigFileLocation != null)
            {
                return OnConfigFileLocation(sender, args);
            }

            return null;
        }

        /// Output message according with logging Level.
        /// </summary>
        /// <param name="log"> log4Net object.</param>
        /// <param name="loggingLevel">The Severity level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">Any exception to be logged.</param>
        /// 
        private static void LogBase(ILog log, Severity loggingLevel, string message, Log4NetException exception)
        {
            if (ShouldLog(log, loggingLevel))
            {
                switch (loggingLevel)
                {
                    case Severity.DEBUG:
                    case Severity.DETAILED_DEBUG:
                        log.Debug(message, exception);
                        break;
                    case Severity.INFO:
                        log.Info(message, exception);
                        break;
                    case Severity.WARNING:
                        log.Warn(message, exception);
                        break;
                    case Severity.ERROR:
                        log.Error(message, exception);
                        break;
                    case Severity.FATAL_ERROR:
                        log.Fatal(message, exception);
                        break;
                }
            }
        }
        /// Check if the logging level is ok.
        /// </summary>
        /// <param name="log"> log4Net object.</param>
        /// <param name="loggingLevel">The Severity level.</param>
        /// 
        private static bool ShouldLog(ILog log, Severity loggingLevel)
        {
            switch (loggingLevel)
            {
                case Severity.DEBUG:
                case Severity.DETAILED_DEBUG:
                    return log.IsDebugEnabled;
                case Severity.INFO: return log.IsInfoEnabled;
                case Severity.WARNING: return log.IsWarnEnabled;
                case Severity.ERROR: return log.IsErrorEnabled;
                case Severity.FATAL_ERROR: return log.IsFatalEnabled;
                default: return false;
            }
        }
        public static void SetLogPath(string logDirectory)
        {
            if (logDirectory.Length <= 0)
                return;
            try
            {
                using (TISMutex = new Mutex(true))
                {
                    bool isLocked = TISMutex.WaitOne(MUTEX_LOCKING_TIMEOUT);

                    if (isLocked)
                    {
                        LogPath = logDirectory;
                        //get the current logging repository for this application 
                        ILoggerRepository repository = LogManager.GetRepository();
                        //get all of the appenders for the repository 
                        IAppender[] appenders = repository.GetAppenders();
                        //only change the file path on the 'FileAppenders' 
                        foreach (IAppender appender in (from iAppender in appenders
                                                        where iAppender is FileAppender
                                                        select iAppender))
                        {
                            FileAppender fileAppender = appender as FileAppender;
                            //set the path to your logDirectory using the original file name defined 
                            //in configuration 
                            fileAppender.File = Path.Combine(logDirectory, Path.GetFileName(fileAppender.File));
                            fileAppender.LockingModel = new FileAppender.MinimalLock();

                            //make sure to call fileAppender.ActivateOptions() to notify the logging 
                            //sub system that the configuration for this appender has changed. 
                            fileAppender.ActivateOptions();
                        }
                    }
                }
            }
            catch
            {
            };
        }

        public static string GetLogPath()
        {
            //get the current logging repository for this application 
            ILoggerRepository repository = LogManager.GetRepository();
            //get all of the appenders for the repository 
            IAppender[] appenders = repository.GetAppenders();
            //only change the file path on the 'FileAppenders' 
            if (appenders.Count() <= 0)
                return string.Empty;

            FileAppender fileAppender = appenders[0] as FileAppender;
            return Path.GetFullPath(fileAppender.File);
        }

        public static void SetMinSeverity(Severity enMinSeverity)
        {
            m_enMinSeverity = enMinSeverity;
        }
        #region OldCode

        /// <summary>
        /// RequestMessageLog old code.
        /// 
        public void RequestMessageLog(string Message, string UnitName, Severity Severity)
        {
            Logger(cfgLogger, Severity, Message, null);
        }

        public void RequestMessageLog(string Message, string UnitName, Severity Severity, Exception exception)
        {
            Logger(cfgLogger, Severity, Message, exception as Log4NetException);
        }

        public static void SetLogFileLocation(string logFilePath)
        {
            if (logFilePath.Length > 0)
                LogPath = logFilePath;
        }

        private static Log4NetException getExceptioObj(ref object[] Params)
        {
            if ((Params == null) || (Params.Length <= 0))
                return null;

            Log4NetException TISExc = null;
            Type obj1 = typeof(Exception);

            object[] newParams = new object[Params.GetLength(0)];
            newParams[Params.GetLength(0) - 1] = string.Empty;

            int index = 0;
            foreach (object oParam in Params)
            {
                if (oParam != null)
                {
                    var obj2 = oParam.GetType();
                    if (obj1.IsAssignableFrom(obj2))
                    {
                        TISExc = new Log4NetException((oParam as Exception).Message, oParam as Exception);
                    }
                    else
                        newParams[index++] = oParam;
                }
            }

            Params = newParams;
            return TISExc;
        }

        private static Log4NetException GetTISException(StackFrame frame, string message = "")
        {
            if (frame == null)
                return null;

            try
            {
                MethodBase method = frame.GetMethod();
                Log4NetException excp = new Log4NetException(message);
                excp.Source = method.Module.Name;
                excp.m_Class = method.DeclaringType.ToString();
                excp.m_Method = method.Name;
                excp.m_StackTrace = frame.ToString();
                return excp;
            }
            catch
            {
                return null;
            }
        }

        private static string GetParams(params object[] Params)
        {
            Log4NetException excepObj = new Log4NetException();
            var obj1 = excepObj.GetType();

            if ((Params == null) || (Params.Length <= 0))
                return string.Empty;

            StringBuilder oMsg = new StringBuilder();
            foreach (object oParam in Params)
            {
                if (oParam != null)
                {
                    var obj2 = oParam.GetType();
                    if (obj1.IsAssignableFrom(obj2))
                        continue;// oMsg.Append((oParam as Exception).Message);
                    else
                        oMsg.Append(oParam.ToString());
                }
                else
                    oMsg.Append("(null)");

                oMsg.Append(" ");
            }

            return oMsg.ToString();
        }

        public static void Write(Severity enSeverity, params object[] Params)
        {
            Log4NetException excp = getExceptioObj(ref Params);

            if (Params != null)
                Logger(cfgLogger, enSeverity, GetParams(Params), excp);
            else
                Logger(cfgLogger, enSeverity, null, excp);
        }

        public static void Write(Severity enSeverity, System.Reflection.MethodBase oMethod, string sFormat, params object[] Params)
        {
            try
            {
                Type declaringType = oMethod.DeclaringType;
                string sTypeName = declaringType.Name;
                string sMethodName = oMethod.Name;
                string tempStr = Params != null ? string.Format(sFormat, Params) : string.Empty;
                string message = string.Format("{0}.{1}:{2}", sTypeName, sMethodName, tempStr);

                Log4NetException excp = getExceptioObj(ref Params);

                Logger(cfgLogger, enSeverity, message, excp);
            }
            catch
            {
                Logger(cfgLogger, enSeverity,
                        string.Format("Log.Write - {0}.{1}:{2}", oMethod.DeclaringType.Name, oMethod.Name),
                        null);
            }
        }

        public static void WriteDebug(string sFormat, params object[] Params)
        {

            try
            {
                string message = System.String.Format(sFormat, Params);
                Log4NetException excp = getExceptioObj(ref Params);

                Logger(cfgLogger, Log.Severity.DEBUG, message, excp);
            }
            catch (Exception e)
            {
                Logger(cfgLogger, Log.Severity.DEBUG, "WriteDebug -", new Log4NetException(e.Message, e));
            }
        }

        public static void WriteDetailedDebug(string sFormat, params object[] Params)
        {
            Log4NetException excp = getExceptioObj(ref Params);

            try
            {
                string message = System.String.Format(sFormat, Params);
                Logger(cfgLogger, Log.Severity.DEBUG, message, excp);
            }
            catch
            {
                Logger(cfgLogger, Log.Severity.DEBUG, "WriteDetailedDebug - " + sFormat, excp);
            }
        }

        public static void WriteInfo(string sFormat, params object[] Params)
        {
            Log4NetException excp = getExceptioObj(ref Params);
            try
            {
                string message = System.String.Format(sFormat, Params);
                Logger(cfgLogger, Log.Severity.INFO, message, excp);
            }
            catch
            {
                Logger(cfgLogger, Log.Severity.INFO, "WriteInfo - " + sFormat, excp);
            }
        }

        public static void WriteWarning(string sFormat, params object[] Params)
        {
            Log4NetException excp = getExceptioObj(ref Params);
            try
            {
                string message = System.String.Format(sFormat, Params);
                Logger(cfgLogger, Log.Severity.WARNING, message, excp);
            }
            catch
            {
                Logger(cfgLogger, Log.Severity.WARNING, "WriteWarning - " + sFormat, excp);
            }
        }

        public static void WriteError(string sFormat, params object[] Params)
        {
            Log4NetException excp = getExceptioObj(ref Params);

            try
            {
                string message = System.String.Format(sFormat, Params);

                Logger(cfgLogger, Log.Severity.ERROR, message, excp);
            }
            catch
            {
                Logger(cfgLogger, Log.Severity.ERROR, "WriteError - " + sFormat, excp);
            }
        }

        public static void WriteFatalError(string sFormat, params object[] Params)
        {
            Log4NetException excp = getExceptioObj(ref Params);
            try
            {
                string message = System.String.Format(sFormat, Params);
                Logger(cfgLogger, Log.Severity.FATAL_ERROR, message, excp);
            }
            catch
            {
                Logger(cfgLogger, Log.Severity.FATAL_ERROR, "WriteFatalError -" + sFormat, excp);
            }
        }

        public static void WriteException(Exception E)
        {
            try
            {
                Logger(cfgLogger, Log.Severity.FATAL_ERROR, string.Empty, new Log4NetException(E.Message, E, new StackFrame(1)));
            }
            catch
            {
                Logger(cfgLogger, Log.Severity.FATAL_ERROR, "WriteException -" + E.Message, E as Log4NetException);
            }
        }

        public static void WriteException(Severity enSeverity, Exception E, params object[] Params)
        {
            try
            {
                string message = GetParams(Params);
                Logger(cfgLogger, Log.Severity.FATAL_ERROR, message, new Log4NetException(E.Message, E, new StackFrame(1)));
            }
            catch
            {
                Logger(cfgLogger, Log.Severity.FATAL_ERROR, "WriteException -" + E.Message, null);
            }
        }
        #endregion
    }
}
