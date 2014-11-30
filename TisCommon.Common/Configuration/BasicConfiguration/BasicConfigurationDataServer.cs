using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using TiS.Core.TisCommon.Network;

namespace TiS.Core.TisCommon.Configuration
{

    /// <summary>
    /// This class contains server side basic configuration parameters
    /// </summary>
    [ComVisible(false)]
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class BasicConfigurationDataServer : IBasicConfigurationDataServer, IDeserializationCallback
    {
        /// <summary>
        /// "Windows" authentication type
        /// </summary>
        public const string AuthenticationType_Windows = "Windows";
        /// <summary>
        /// "User" authentication type
        /// </summary>
		public const string AuthenticationType_User = "User";

        /// <summary>
        /// The default appdata folder
        /// </summary>
        private const string DEFAULT_APPDATA_FOLDER = "Server";
        /// <summary>
        /// The local sql server
        /// </summary>
        private const string LOCAL_SQL_SERVER = "(local)";
        /// <summary>
        /// The default management db name
        /// </summary>
        private const string DEFAULT_DB_NAME = "eFlow_Management";
        /// <summary>
        /// The default monitor db name
        /// </summary>
        private const string DEFAULT_MONITOR_DB_NAME = "eFlow_Monitor";
        /// <summary>
        /// The default station liveliness timeout
        /// </summary>
        private const int DEFAULT_STATION_LIVELINESS_TIMEOUT = 10 * 60 * 1000; // 10 min.
        /// <summary>
        /// The default resource disposer interval
        /// </summary>
        private const int DEFAULT_RESOURCE_DISPOSER_INTERVAL = 60 * 1000; // 60 sec.
        /// <summary>
        /// The default maximum tasks count
        /// </summary>
        private const int DEFAULT_MAX_TASKS_COUNT = 100;
        /// <summary>
        /// The default task life timeout
        /// </summary>
        private const int DEFAULT_TASK_LIFE_TIMEOUT = 10 * 60 * 1000; // 10 min. (2 * DEFAULT_TASK_POLLING_INTERVAL) 
        /// <summary>
        /// The default authentication type
        /// </summary>
		private const string DEFAULT_AUTHENTICATION_TYPE = AuthenticationType_Windows;

        /// <summary>
        /// The default alert generation interval
        /// </summary>
        private const int DEFAULT_ALERT_GENERATE_INTERVAL = 60 * 1000; // 1 min

        //SQL server
        //TODO : use encrypted section / move to special section
        /// <summary>
        /// The application data path
        /// </summary>
        private string m_applicationDataPath = DEFAULT_APPDATA_FOLDER;
        /// <summary>
        /// The m_local SQL server
        /// </summary>
        private string m_localSQLServer = LOCAL_SQL_SERVER;
        /// <summary>
        /// Indicates whether the integrated security is used
        /// </summary>
        private bool m_integratedSecurity = false;
    
        private string m_dbName = DEFAULT_DB_NAME;
        private string m_dbMonitorName = DEFAULT_MONITOR_DB_NAME;
        /// <summary>
        /// The DB user name
        /// </summary>
        private string m_dbUserName = "sa";
        /// <summary>
        /// The DB password
        /// </summary>
        private string m_dbPassword = "sa";
        /// <summary>
        /// The db authentication type
        /// </summary>
        private string m_authenticationType;

        /// <summary>
        /// Indicates whether to use the SQL dynamic storage
        /// </summary>
        private bool m_UseSqlDynamicStorage = false;

        /// <summary>
        /// The maximum pool size
        /// </summary>
        private int m_MaxPoolSize = 1000;
        /// <summary>
        /// The SQL connection timeout
        /// </summary>
        private int m_SqlConnectionTimeout = 60;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConfigurationDataServer"/> class.
        /// </summary>
        public BasicConfigurationDataServer()
        {
            StationLivelinessTimeout = DEFAULT_STATION_LIVELINESS_TIMEOUT;
            ResourceDisposerInterval = DEFAULT_RESOURCE_DISPOSER_INTERVAL;
            MaxTasksCount = DEFAULT_MAX_TASKS_COUNT;
            TaskLifeTimeout = DEFAULT_TASK_LIFE_TIMEOUT;
            m_authenticationType = DEFAULT_AUTHENTICATION_TYPE;

            AlertsGenerateInterval = DEFAULT_ALERT_GENERATE_INTERVAL;

            CachePort = 22233;
            CacheHost = "localhost";
            CacheApplication = "default";
        }

        #region SQL server

        /// <summary>
        /// Gets or sets the maximum size of the pool.
        /// </summary>
        /// <value>
        /// The maximum size of the pool.
        /// </value>
        [DataMember]
        public int MaxPoolSize 
        {
            get { return m_MaxPoolSize; }
            set { m_MaxPoolSize = value; } 
        }

        /// <summary>
        /// Gets or sets the SQL connection timeout.
        /// </summary>
        /// <value>
        /// The SQL connection timeout.
        /// </value>
        [DataMember]
        public int SqlConnectionTimeout 
        {
            get { return m_SqlConnectionTimeout; }
            set { m_SqlConnectionTimeout = value; } 
        }

        /// <summary>
        /// Gets and sets the property indicating whether dynamic data will be saved on the file system or SQL server storage.
        /// </summary>
        /// <value>
        /// Default value is <b>false</b>
        /// </value>
        [DataMember]
        public bool UseSqlDynamicStorage
        {
            get { return m_UseSqlDynamicStorage; }
            set { m_UseSqlDynamicStorage = value; }
        }

        /// <summary>
        /// Gets and sets the SQL server name
        /// </summary>
        /// <remark>Property should contain the full SQL server instance name.</remark>
        [DataMember]
        public string LocalSQLServer
        {
            get { return m_localSQLServer; }
            set { m_localSQLServer = value; }
        }

        /// <summary>
        /// Gets or sets the name of the eFLOW management database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        [DataMember]
        public string DBName
        {
            get { return m_dbName; }
            set { m_dbName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the eFLOW Monitor database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        [DataMember]
        public string MonitorDBName
        {
            get { return m_dbMonitorName; }
            set { m_dbMonitorName = value; }
        }

        

        /// <summary>
        /// Gets and sets the SQL server user name.
        /// </summary>
        /// <remarks>
        /// Relevant only if <see cref="IntegratedSecurity" /> is <b>false</b>.
        /// </remarks>
        [DataMember]
        public string DBUserName
        {
            get { return m_dbUserName; }
            set { m_dbUserName = value; }
        }

        //TODO : encryption
        /// <summary>
        /// Gets and sets the SQL server user password.
        /// </summary>
        /// <remarks>
        /// Relevant only if <see cref="IntegratedSecurity" /> is <b>false</b>.
        /// </remarks>
        [DataMember]
        public string DBPassword
        {
            get { return m_dbPassword; }
            set { m_dbPassword = value; }
        }

        //TODO : encryption
        /// <summary>
        /// Gets and sets the property indicating whether integrated security will be used for SQL connections.
        /// </summary>
        [DataMember]
        public bool IntegratedSecurity
        {
            get { return m_integratedSecurity; }
            set { m_integratedSecurity = value; }
        }
        
        #endregion

        #region Management

        /// <summary>
        /// Gets or sets the station liveliness timeout.
        /// </summary>
        /// <value>
        /// The station liveliness timeout.
        /// </value>
        [DataMember]
        public int StationLivelinessTimeout { get; set; }

        /// <summary>
        /// Gets or sets the resource disposer interval.
        /// </summary>
        /// <value>
        /// The resource disposer interval.
        /// </value>
        [DataMember]
        public int ResourceDisposerInterval { get; set; }

        /// <summary>
        /// Gets or sets the maximum tasks count.
        /// </summary>
        /// <value>
        /// The maximum tasks count.
        /// </value>
        [DataMember]
        public int MaxTasksCount { get; set; }

        /// <summary>
        /// Gets or sets the task life timeout.
        /// </summary>
        /// <value>
        /// The task life timeout.
        /// </value>
        [DataMember]
        public int TaskLifeTimeout { get; set; }

        /// <summary>
        /// Gets or sets the alerts generation interval.
        /// </summary>
        /// <value>
        /// The alerts generate interval.
        /// </value>
        [DataMember]
        public int AlertsGenerateInterval { get; set; }
        #endregion

		#region Security

        /// <summary>
        /// Gets or sets the authentication type.
        /// </summary>
        /// <value>
        /// The type of the authentication.
        /// </value>
		[DataMember]
		public string AuthenticationType 
        {
            get
            {
                return m_authenticationType;
            }
            set
            {
                m_authenticationType = value.Trim();
            }
        }
	
		#endregion

        #region Cache
        /// <summary>
        /// AppFabric cache port
        /// </summary>
        [DataMember]
        public int CachePort { get; set; }
        /// <summary>
        /// AppFabric cache host
        /// </summary>
        [DataMember]
        public string CacheHost { get; set; }
        /// <summary>
        /// AppFabric application name
        /// </summary>
        [DataMember]
        public string CacheApplication { get; set; }
        #endregion

        #region IDeserializationCallback Members

        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
		public void OnDeserialization(object sender)
        {
            EnsureDefaultValues();
        }

        /// <summary>
        /// Ensures the default values.
        /// </summary>
        private void EnsureDefaultValues()
        {
            if (!StringUtil.IsStringInitialized(this.m_applicationDataPath))
            {
                this.m_applicationDataPath = DEFAULT_APPDATA_FOLDER;
            }

            if (!StringUtil.IsStringInitialized(this.m_localSQLServer))
            {
                this.m_localSQLServer = LOCAL_SQL_SERVER;
            }

            if (!StringUtil.IsStringInitialized(this.m_dbName))
            {
                this.m_dbName = DEFAULT_DB_NAME;
            }

            if (!StringUtil.IsStringInitialized(this.m_dbMonitorName))
            {
                this.m_dbMonitorName = DEFAULT_MONITOR_DB_NAME;
            }

            if (!StringUtil.IsStringInitialized(this.m_dbUserName))
            {
                this.m_dbUserName = StringUtil.NullToEmpty(this.m_dbUserName);
                this.IntegratedSecurity = true;
            }

            if (!StringUtil.IsStringInitialized(this.m_dbPassword))
            {
                this.m_dbPassword = StringUtil.NullToEmpty(this.m_dbPassword);
                this.IntegratedSecurity = true;
            }

            if (this.StationLivelinessTimeout == 0)
            {
                this.StationLivelinessTimeout = DEFAULT_STATION_LIVELINESS_TIMEOUT;
            }

            if (this.ResourceDisposerInterval == 0)
            {
                this.ResourceDisposerInterval = DEFAULT_RESOURCE_DISPOSER_INTERVAL;
            }

            if (this.MaxTasksCount == 0)
            {
                MaxTasksCount = DEFAULT_MAX_TASKS_COUNT;
            }

            if (this.TaskLifeTimeout == 0)
            {
                TaskLifeTimeout = DEFAULT_TASK_LIFE_TIMEOUT;
            }

			if (this.AlertsGenerateInterval == 0)
			{
				AlertsGenerateInterval = DEFAULT_ALERT_GENERATE_INTERVAL;
			}

			if (!StringUtil.IsStringInitialized(this.AuthenticationType))
			{
				this.AuthenticationType = DEFAULT_AUTHENTICATION_TYPE;
			}

            if (String.IsNullOrEmpty(this.CacheApplication))
            {          
                CacheApplication = "default";
            }

            if (String.IsNullOrEmpty(this.CacheHost))
            {
                CacheHost = "localhost";
            }

            if (this.CachePort == 0)
            {
                CachePort = 22233;
            }
		}

        #endregion

        
    }
 
}
