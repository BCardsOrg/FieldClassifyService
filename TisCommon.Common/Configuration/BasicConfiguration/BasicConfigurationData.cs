using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using TiS.Core.TisCommon.Network;
using TiS.Core.TisCommon.Services;
using System.Web.Hosting;

namespace TiS.Core.TisCommon.Configuration
{
    /// <summary>
    ///This class contains eFLOW configuration data 
    /// </summary>
    [ComVisible(false)]
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class BasicConfigurationData : IBasicConfigurationData, IDeserializationCallback
    {
        /// <summary>
        /// The bin path
        /// </summary>
        private const string BIN_PATH = "Bin";
        /// <summary>
        /// The temporary  path
        /// </summary>
        private const string TEMP_PATH = "Temp";
        /// <summary>
        /// The defaul task polling interval
        /// </summary>
        private const int DEFAULT_TASK_POLLING_INTERVAL = 5 * 60 * 1000; // 5  min  in [msec].
        /// <summary>
        /// The default autorunstarter reconnection interval
        /// </summary>
        private const int DEFAULT_AUTORUNSTARTER_RECONNECTION_INTERVAL = 5 * 60 * 1000; // 5 min.
        /// <summary>
        /// The default collection export/import chunk size
        /// </summary>
        private const int DEFAULT_COLLECTIONS_EXPORT_IMPORT_CHUNK_SIZE = 200;
        /// <summary>
        /// The default collection chunk size
        /// </summary>
        private const int DEFAULT_COLLECTIONS_CHUNK_SIZE = 2000000;
        /// <summary>
        /// The default huge collection threshold
        /// </summary>
        private const int DEFAULT_HUGE_COLLECTIONS_THRESHOLD = 10000000;

        /// <summary>
        /// The default demo mode
        /// </summary>
		private const bool DEFAULT_DEMO_MODE = false;

        /// <summary>
        /// The default performanc counter interval
        /// </summary>
        private const int DEFAULT_PERFORMANCE_COUNTER_INTERVAL = 5 * 60 * 1000; // 5 min
        /// <summary>
        /// The default monitor autorun stations interval
        /// </summary>
        private const int DEFAULT_MONITOR_AUTORUN_STATIONS_INTERVAL = 10 * 60 * 1000; // 10 min.

        /// <summary>
        /// The default path
        /// </summary>
        private const string DEFAULT_PATH = @"c:\DEFAULT";

        /// <summary>
        /// The server machine name
        /// </summary>
        private string m_serverMachineName = "";

        /// <summary>
        /// The communication protocol
        /// </summary>
        private string m_CommProtocol = TisServicesConst.DEFAULT_COMMUNICATION_PROTOCOL;
        /// <summary>
        /// The m_ comm port
        /// </summary>
        private int m_CommPort = TisServicesConst.DEFAULT_COMMUNICATION_PORT;
        /// <summary>
        /// The m_ local node name
        /// </summary>
        private string m_LocalNodeName = MachineInfo.MachineName;
        /// <summary>
        /// The m_ local machine name
        /// </summary>
        private string m_LocalMachineName = MachineInfo.MachineName;

        // Base eFLOW Path definitions
        /// <summary>
        /// The m_e flow install path
        /// </summary>
        private string m_eFlowInstallPath = Path.Combine(DEFAULT_PATH, "..");
        /// <summary>
        /// The m_e flow data path
        /// </summary>
        private string m_eFlowDataPath = "";
        /// <summary>
        /// The m_e flow document rs path
        /// </summary>
        private string m_eFlowOCRsPath = Path.Combine(DEFAULT_PATH, "OCRs");
        /// <summary>
        /// The m_e flow database engines path
        /// </summary>
        private string m_eFlowDBEnginesPath = Path.Combine(DEFAULT_PATH, "DBEngines");
        /// <summary>
        /// The m_lang path
        /// </summary>
        private string m_langPath = Path.Combine(DEFAULT_PATH, "Language");
        /// <summary>
        /// m_enableEquatechStatistics - false in QA mode
        /// </summary>
        private bool m_enableEquatechStatistics = true;

        private bool? m_saveUserAuthentication = true;

        /// <summary>
        /// The collections chunk size
        /// </summary>
        private int m_collectionsChunkSize;
        /// <summary>
        /// The huge collections threshold
        /// </summary>
        private int m_hugeCollectionsThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConfigurationData"/> class.
        /// </summary>
        public BasicConfigurationData()
        {
            TaskPollingInterval = DEFAULT_TASK_POLLING_INTERVAL;
            AutorunStarterReconnectionInterval = DEFAULT_AUTORUNSTARTER_RECONNECTION_INTERVAL;
            CollectionsExportImportChunkSize = DEFAULT_COLLECTIONS_EXPORT_IMPORT_CHUNK_SIZE;
            CollectionsChunkSize = DEFAULT_COLLECTIONS_CHUNK_SIZE;
            HugeCollectionsThreshold = DEFAULT_HUGE_COLLECTIONS_THRESHOLD;

            PerformanceCounterInterval = DEFAULT_PERFORMANCE_COUNTER_INTERVAL;
			
			DemoMode = DEFAULT_DEMO_MODE;

            MonitorAutorunStationsInterval = DEFAULT_MONITOR_AUTORUN_STATIONS_INTERVAL;
        }
      
        #region Machine configuration

        /// <summary>
        /// Gets and sets the server machine name
        /// </summary>
        [DataMember]
        public string ServerMachineName
        {
            get { return m_serverMachineName; }
            set 
            { 
                m_serverMachineName = value;
            }
        }

        /// <summary>
        /// Gets and sets the  communication protocol.
        /// </summary>
        /// <value>
        /// Default value is <b>http</b>
        /// </value>
        [DataMember]
        public string CommunicationProtocol
        {
            get { return m_CommProtocol; }
            set { m_CommProtocol = value.Trim(); }
        }

        /// <summary>
        /// Gets and sets the  communication port used by the communication protocol defined by the <see cref="CommunicationProtocol" /> property.
        /// </summary>
        /// <value>
        /// Default value is <b>55222</b>
        /// </value>
        [DataMember]
        public int CommunicationPort
        {
            get { return m_CommPort; }
            set { m_CommPort = value; }
        }

        /// <summary>
        /// Gets and sets the local machine name
        /// </summary>
        /// <remarks>
        /// In standalone installation the server and local machine names are identical.
        /// </remarks>
        [DataMember]
        public string LocalMachineName
        {
            get 
            {
                return m_LocalMachineName = m_LocalMachineName ?? MachineInfo.MachineName;
            }

            set { m_LocalMachineName = value.Trim(); }
        } 
        #endregion

        #region eFlow paths

        /// <summary>
        /// Gets and sets the path to eFLOW data.
        /// </summary>
        [DataMember]
        public string eFlowDataPath
        {
            get { return m_eFlowDataPath; }
            set { m_eFlowDataPath = value.Trim();}
        }

        /// <summary>
        /// Gets and sets the path to eFLOW client installation.
        /// </summary>
		[DataMember]
        public string eFlowInstallPath
        {
            get { return m_eFlowInstallPath; }
            set { m_eFlowInstallPath = value.Trim(); }
        }

        /// <summary>
        /// Gets and sets the path to eFLOW OCR engines installation.
        /// </summary>
        [DataMember]
        public string eFlowOCRsPath
        {
            get { return m_eFlowOCRsPath; }
            set { m_eFlowOCRsPath = value.Trim(); }
        }

        /// <summary>
        /// Gets and sets the path to eFLOW localization files directory.
        /// </summary>
        [DataMember]
        public string eFlowLangPath
        {
            get { return m_langPath; }
            set { m_langPath = value.Trim(); }
        }

        /// <summary>
        /// Gets and sets the path to eFLOW DB Engines installation.
        /// </summary>
        [DataMember]
        public string eFlowDBEnginesPath
        {
            get { return m_eFlowDBEnginesPath; }
            set { m_eFlowDBEnginesPath = value.Trim(); }
        }

        /// <summary>
        /// Gets or sets the task polling interval.
        /// </summary>
        /// <value>
        /// The task polling interval.
        /// </value>
        [DataMember]
        public int TaskPollingInterval { get; set; }

        /// <summary>
        /// Gets or sets the autorun starter reconnection interval.
        /// </summary>
        /// <value>
        /// The autorun starter reconnection interval.
        /// </value>
        [DataMember]
        public int AutorunStarterReconnectionInterval { get; set; }

        /// <summary>
        /// Gets or sets the size of the collections export import chunk.
        /// </summary>
        /// <value>
        /// The size of the collections export import chunk.
        /// </value>
        [DataMember]
        public int CollectionsExportImportChunkSize { get; set; }

        /// <summary>
        /// Gets or sets whether to enable or disable Equatech statistics
        /// </summary>
        /// <value>
        /// true if enable Equatech statistics, false otherwise.
        /// </value>
        [DataMember]
        public bool EnableEquatechStatistics
        {
            get{ return m_enableEquatechStatistics; }
            set{ m_enableEquatechStatistics = value;}
        }

        /// <summary>
        /// Gets or sets the size of the collections chunk.
        /// </summary>
        /// <value>
        /// The size of the collections chunk.
        /// </value>
        [DataMember]
        public int CollectionsChunkSize 
        { 
            get {return m_collectionsChunkSize;}
            set { m_collectionsChunkSize = value ; } 
        }

        /// <summary>
        /// Gets or sets the huge collections threshold.
        /// </summary>
        /// <value>
        /// The huge collections threshold.
        /// </value>
        [DataMember]
        public int HugeCollectionsThreshold
        {
            get { return m_hugeCollectionsThreshold; }
            set { m_hugeCollectionsThreshold = value ; }
        }

        /// <summary>
        /// Gets or sets the time interval that defines how often the performance counters will be checked. Used by Supervise station only.
        /// </summary>
        /// <value>
        /// The performance counter interval.
        /// </value>
        /// <remarks>For internal use only.</remarks>
        [DataMember]
        public int PerformanceCounterInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether eFLOW should run in demo mode.
        /// </summary>
        /// <remarks>In demo mode all the time intervals are set to minimal values. This configuration provides fast response to any changes in the system, but is not recommended to use in production environment, since it puts high load on the server.</remarks>
		[DataMember]
		public bool DemoMode { get; set; }
		#endregion

        /// <summary>
        /// Gets the path to the eFLOW client binaries directory.
        /// </summary>
        /// <remarks>
        /// Used internally
        /// </remarks>
        public string eFlowBinPath
        {
            get
            {
                string eFlowAppPath = HostingEnvironment.ApplicationPhysicalPath ?? eFlowInstallPath;

                return Path.Combine(eFlowAppPath, BIN_PATH);
            }
        }

        /// <summary>
        /// Gets the path to the eFLOW temporary directory
        /// </summary>
        /// <remarks>
        /// Used internally
        /// </remarks>
        public string eFlowTempPath
        {
            get
            {
                return Path.Combine(eFlowDataPath, TEMP_PATH);
            }
        }

        /// <summary>
        /// Gets or sets the interval used to monitor autorun stations status.
        /// </summary>
        /// <value>
        /// The interval used to monitor autorun stations status.
        /// </value>
        [DataMember]
        public int MonitorAutorunStationsInterval { get; set; }

        /// <summary>
        /// if true save the user login name & password in local machine, so next time login to User & Password will be automatic
        /// </summary>
        [DataMember]
        public bool SaveUserAuthentication
        {
            get { return m_saveUserAuthentication.Value; }
            set { m_saveUserAuthentication = value; }
        }

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
            if (this.TaskPollingInterval == 0)
            {
                this.TaskPollingInterval = DEFAULT_TASK_POLLING_INTERVAL;
            }

            if (this.AutorunStarterReconnectionInterval == 0)
            {
                this.AutorunStarterReconnectionInterval = DEFAULT_AUTORUNSTARTER_RECONNECTION_INTERVAL;
            }

            if (this.CollectionsExportImportChunkSize == 0)
            {
                this.CollectionsExportImportChunkSize = DEFAULT_COLLECTIONS_EXPORT_IMPORT_CHUNK_SIZE;
            }

            if (this.EnableEquatechStatistics == false)
            {
                this.EnableEquatechStatistics = true;
            }

            if (this.CollectionsChunkSize == 0)
            {
                this.CollectionsChunkSize = DEFAULT_COLLECTIONS_CHUNK_SIZE;
            }

            if (this.HugeCollectionsThreshold == 0)
            {
                this.HugeCollectionsThreshold = DEFAULT_HUGE_COLLECTIONS_THRESHOLD;
            }

            if (this.MonitorAutorunStationsInterval == 0)
            {
                this.MonitorAutorunStationsInterval = DEFAULT_MONITOR_AUTORUN_STATIONS_INTERVAL;
            }

            if (this.m_saveUserAuthentication.HasValue == false)
            {
                m_saveUserAuthentication = true;
            }
        }

        #endregion


    }
}
