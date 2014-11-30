using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using TiS.Core.TisCommon.Network;
using System.IO;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Cache;

namespace TiS.Core.TisCommon.Configuration
{

    [Guid("50EC2F24-0D81-4C74-8724-E4BB9A6CFABC")]
    public interface IBasicConfiguration : IBasicConfigurationData, IConfigurationActions
    {
    }

    [ComVisible(false)]
    public class BasicConfiguration : IBasicConfiguration
    {
        /// <summary>
        /// The log path
        /// </summary>
        private const string LOG_PATH = "Log";

        /// <summary>
        /// The configuration data
        /// </summary>
        private BasicConfigurationData m_data;
        /// <summary>
        /// The configuration provider
        /// </summary>
        private IBasicConfigurationProvider m_provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConfiguration"/> class.
        /// </summary>
        public BasicConfiguration()
        {
            m_provider = new BasicConfigurationProvider();

            Load();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicConfiguration"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        internal BasicConfiguration( IBasicConfigurationProvider provider )
        {
            m_provider = provider;

            Load();
        }

        /// <summary>
        /// Loads the current instance of the <see cref="BasicConfiguration"/> class.
        /// </summary>
        public void Load()
        {
            try
            {
                m_data = m_provider.Load();

            }
            catch (Exception exc)
            {
                Log.WriteError("Failed loading configuration : {0}", exc);
            }
        }

        /// <summary>
        /// Saves the current instance of the <see cref="BasicConfiguration"/> class.
        /// </summary>
        public void Save()
        {
            if (m_data == null)
            {
                return;
            }
            try
            {
                m_provider.Save(m_data);
            }
            catch (Exception exc)
            {
                Log.WriteError("Failed saving configuration : {0}", exc);
            }
        }

        #region Basic configuration data

        #region Machine configuration

        /// <summary>
        /// Gets and sets the server machine name
        /// </summary>
        public string ServerMachineName
        {
            get { return m_data.ServerMachineName; }
            set { m_data.ServerMachineName = value; }
        }

        /// <summary>
        /// Gets and sets the  communication protocol.
        /// </summary>
        /// <value>
        /// Default value is <b>http</b>
        /// </value>
        public string CommunicationProtocol
        {
            get { return m_data.CommunicationProtocol; }
            set { m_data.CommunicationProtocol = value; }
        }

        /// <summary>
        /// Gets and sets the  communication port used by the communication protocol defined by the <see cref="CommunicationProtocol" /> property.
        /// </summary>
        /// <value>
        /// Default value is <b>55222</b>
        /// </value>
        public int CommunicationPort
        {
            get { return m_data.CommunicationPort; }
            set { m_data.CommunicationPort = value; }
        }

        /// <summary>
        /// Gets and sets the local machine name
        /// </summary>
        /// <remarks>
        /// In standalone installation the server and local machine names are identical.
        /// </remarks>
        public string LocalMachineName
        {
            get { return m_data.LocalMachineName; }
            set { m_data.LocalMachineName = value; }
        }

        /// <summary>
        /// Gets or sets the task polling interval.
        /// </summary>
        /// <value>
        /// The task polling interval.
        /// </value>
        public int TaskPollingInterval
        {
            get { return (DemoMode == true) ? 1000 : m_data.TaskPollingInterval; }
            set { m_data.TaskPollingInterval = value; }
        }

        /// <summary>
        /// Gets or sets the performance counter interval.
        /// </summary>
        /// <value>
        /// The performance counter interval.
        /// </value>
        public int PerformanceCounterInterval 
        {
            get { return (DemoMode == true) ? 10000 : m_data.PerformanceCounterInterval; }
            set { m_data.PerformanceCounterInterval = value; }    
        }

        /// <summary>
        /// Gets or sets a value indicating whether eFLOW is running in demo mode.
        /// </summary>
		public bool DemoMode
		{
			get { return m_data.DemoMode; }
			set { m_data.DemoMode = value; }
		}

        /// <summary>
        /// Gets or sets the autorun starter reconnection interval.
        /// </summary>
        /// <value>
        /// The autorun starter reconnection interval.
        /// </value>
		public int AutorunStarterReconnectionInterval
        {
            get { return m_data.AutorunStarterReconnectionInterval; }
            set { m_data.AutorunStarterReconnectionInterval = value; }
        }

        /// <summary>
        /// Gets or sets the size of the collection export/import chunk.
        /// </summary>
        /// <value>
        /// The size of the collection export/import chunk.
        /// </value>
        public int CollectionsExportImportChunkSize
        {
            get { return m_data.CollectionsExportImportChunkSize; }
            set { m_data.CollectionsExportImportChunkSize = value; }
        }

        /// <summary>
        /// Gets or sets the size of the collection chunk.
        /// </summary>
        /// <value>
        /// The size of the collection chunk.
        /// </value>
        public int CollectionsChunkSize
        {
            get { return m_data.CollectionsChunkSize; }
            set { m_data.CollectionsChunkSize = value; }
        }

        /// <summary>
        /// Gets or sets whether to enable or disable Equatech statistics
        /// </summary>
        /// <value>
        /// true if enable Equatech statistics, false otherwise.
        /// </value>
        public bool EnableEquatechStatistics
        {
            get { return m_data.EnableEquatechStatistics; }
            set { m_data.EnableEquatechStatistics = value; }
        }

        /// <summary>
        /// Gets or sets the huge collections threshold.
        /// </summary>
        /// <value>
        /// The huge collections threshold.
        /// </value>
        public int HugeCollectionsThreshold
        {
            get { return m_data.HugeCollectionsThreshold; }
            set { m_data.HugeCollectionsThreshold = value; }
        }


        /// <summary>
        /// Gets or sets the time interval used to monitor autorun stations.
        /// </summary>
        /// <value>
        /// The time interval used to monitor autorun stations.
        /// </value>
        public int MonitorAutorunStationsInterval
        {
            get
            {
                return (DemoMode == true || m_data.MonitorAutorunStationsInterval < 30000) ? 30000 : m_data.MonitorAutorunStationsInterval;
            }
            set
            {
                m_data.MonitorAutorunStationsInterval = value;
            }
        }

        #endregion

        #region eFlow paths

        /// <summary>
        /// Gets and sets the path to eFLOW data.
        /// </summary>
        public string eFlowDataPath
        {
            get { return m_data.eFlowDataPath; }
            set
            {
                if (m_data.eFlowDataPath != value)
                {
                    // Set new Data path
                    m_data.eFlowDataPath = value;

                    // Update log path
                    SetLogPathFromDataPath();

                    // Load data from new path
                    Load();

                    //// Set data path again - important after load
                    m_data.eFlowDataPath = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the path to eFLOW client installation.
        /// </summary>
        public string eFlowInstallPath
        {
            get { return m_data.eFlowInstallPath; }
            set { m_data.eFlowInstallPath = value; }
        }

        /// <summary>
        /// Gets and sets the path to eFLOW OCR engines installation.
        /// </summary>
        public string eFlowOCRsPath
        {
            get { return m_data.eFlowOCRsPath; }
            set { m_data.eFlowOCRsPath = value; }
        }

        /// <summary>
        /// Gets and sets the path to eFLOW localization files directory.
        /// </summary>
        public string eFlowLangPath
        {
            get { return m_data.eFlowLangPath; }
            set { m_data.eFlowLangPath = value; }
        }

        /// <summary>
        /// Gets and sets the path to eFLOW DB Engines installation.
        /// </summary>
        public string eFlowDBEnginesPath
        {
            get { return m_data.eFlowDBEnginesPath; }
            set { m_data.eFlowDBEnginesPath = value; }
        }

        #endregion

        #region Calculated values

        /// <summary>
        /// Gets the path to eFLOW binaries directory.
        /// </summary>
        /// <remarks>
        /// Used internally
        /// </remarks>
        public string eFlowBinPath
        {
            get { return m_data.eFlowBinPath; }
        }

        /// <summary>
        /// Gets the path to <i>eFLOW</i> temp directory
        /// </summary>
        /// <remarks>
        /// Used internally
        /// </remarks>
        public string eFlowTempPath
        {
            get { return m_data.eFlowTempPath; }
        }

        #endregion

        /// <summary>
        /// Sets the log path using the data path.
        /// </summary>
        private void SetLogPathFromDataPath()
        {
            string logPath = Path.Combine(
                eFlowDataPath,
                LOG_PATH);

            Log.SetLogFileLocation(logPath);
        }

        #endregion

        public bool SaveUserAuthentication
        {
            get { return m_data.SaveUserAuthentication; }
            set { m_data.SaveUserAuthentication = value; }
        }

    }
}
