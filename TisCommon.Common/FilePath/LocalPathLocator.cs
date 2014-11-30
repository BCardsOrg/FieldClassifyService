using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.FilePath
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(false)]
	public class LocalPathLocator: ITisClientPathLocator, IPathProvider, IDisposable
	{
        private Dictionary<string, string> m_pathMap;
        private string m_applicationName;
        private string m_nodeName;
		private const string DEFAULT_ROOT = "%eFlowData%\\Client\\<ApplicationName>\\";
        private const string WEB_DATA_FOLDER = "WebData\\<UserName>";

        private const string EFLOW_WORK_DIR = "WorkDir";

        private const string APP_NAME_VARIABLE      = "<ApplicationName>";
        private const string USER_NAME_VARIABLE = "<UserName>";
        protected const string EFLOW_DATA_VARIABLE = "%eFlowData%";
		protected const string EFLOW_VARIABLE = "%eFlow%";
		protected const string EFLOW_OCRS_VARIABLE  = "%eFlowOCRs%";
		protected const string EFLOW_TEMP_VARIABLE  = "%eFlowTemp%";
		protected const string STATION_VARIABLE     = "%Station%";

		protected BasicConfiguration m_basicConfiguration;

        private bool m_isWebSession;
        private string m_WebDataUserStorageFolder;

        public LocalPathLocator(
            BasicConfiguration basicConfiguration,
            string sNodeName,
            string sAppName,
            string userName = "",
            bool isWebSession = false)
            : this(basicConfiguration, sAppName)
        {
            m_nodeName = sNodeName;

            m_isWebSession = isWebSession && string.IsNullOrEmpty(userName) == false;

            if (m_isWebSession)
            {
                m_WebDataUserStorageFolder = WEB_DATA_FOLDER;
                m_WebDataUserStorageFolder = userName;
            }

            Load();

            EnsureDefinedPathTargetsExist();
        }

        public LocalPathLocator(
            BasicConfiguration basicConfiguration,
            string sAppName)
        {
            m_basicConfiguration = basicConfiguration;
            m_applicationName = sAppName;
        }

		public string this[string sExtension] 
		{
			get
			{
				string sPath = GetPathForExtension(sExtension);

				EnsurePathTargetExist(sPath);

				return sPath;
			}
		}

		public string Path(string sExtension)
		{
			return this[sExtension];
		}

        public string GetPath(string sExtension)
        {
            return this[sExtension];
        }

        public Dictionary<string, string> PathMap 
		{ 
			get
			{
				return m_pathMap;
			}
		}

		public void SavePathMap()
		{
		}

        public string DefaultRoot
        {
            get
            {
                return GetFullPathForSubPath(String.Empty);
            }
        }

        public string WebDataBaseFolder
        {
            get
            {
                return System.IO.Path.Combine(DefaultRoot, WEB_DATA_FOLDER);
            }
        }

		protected virtual void ReplaceKnownVariables(ref string sPathTemplate)
		{
			// eFlowData
			sPathTemplate = sPathTemplate.Replace(
				EFLOW_DATA_VARIABLE, 
				eFlowDataPath);

			// eFlow
			sPathTemplate = sPathTemplate.Replace(
				EFLOW_VARIABLE, 
				eFlowPath);
			
			// eFlowOCRs
			sPathTemplate = sPathTemplate.Replace(
				EFLOW_OCRS_VARIABLE, 
				eFlowOCRsPath);

			// eFlowTemp
			sPathTemplate = sPathTemplate.Replace(
				EFLOW_TEMP_VARIABLE, 
				eFlowTempPath);

			// %Station%
			sPathTemplate = sPathTemplate.Replace(
				STATION_VARIABLE, 
				m_nodeName);
			
			// <ApplicationName>
			sPathTemplate = sPathTemplate.Replace(
				APP_NAME_VARIABLE, 
				m_applicationName);
		}

        private void Load()
        {
            string resourceFileName = "TiS.Core.TisCommon.Resources.PathMappings.xml";

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = thisAssembly.GetManifestResourceStream(resourceFileName);

            if (resourceStream != null)
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, string>));

                m_pathMap = new Dictionary<string, string>((Dictionary<string, string>)serializer.ReadObject(resourceStream));
            }
            else
            {
                throw new TisException("Failed to load path map.");
            }
        }

        private string GetPathForExtension(string sExtension)
        {
            // Get path template from map

            string sSubPath;


            PathMap.TryGetValue(sExtension, out sSubPath);

            // Check if valid
            if (sSubPath == null)
            {
                // Use default
                sSubPath = GetDefaultPathMapping(sExtension);
            }

            if (StringUtil.CompareIgnoreCase(sSubPath, EFLOW_WORK_DIR) == true)
            {
                if (StringUtil.CompareIgnoreCase(sExtension, "XML") == false)
                {
                    if (m_isWebSession == true)
                    {
                        sSubPath = System.IO.Path.Combine(sSubPath, m_WebDataUserStorageFolder);
                    }
                    sSubPath = System.IO.Path.Combine(sSubPath, ProcessUtil.UniqueProcessId);
                }
            }

            string sFullPath =
                GetFullPathForSubPath(sSubPath);

            if (!sFullPath.EndsWith(@"\"))
            {
                sFullPath += @"\";
            }

            return sFullPath;
        }

		private string GetFullPathForSubPath(string sSubPath)
		{
			string sFullPath = DEFAULT_ROOT + sSubPath;

			// Replace known variables 
			ReplaceKnownVariables(ref sFullPath);

			return sFullPath;
		}

		private	void EnsureDefinedPathTargetsExist()
		{
			foreach(string sPath in m_pathMap.Values)
			{
				EnsurePathTargetExist(
					GetFullPathForSubPath(sPath));
			}
		}

		private void EnsurePathTargetExist(string sFullPath)
		{
			PathUtil.EnsurePathExist(sFullPath);
		}

		private string GetDefaultPathMapping(string sExtension)
		{
			return sExtension;
		}
		
		private string eFlowDataPath
		{
			get { return NormalizePath(m_basicConfiguration.eFlowDataPath ); }
		}

		private string eFlowPath
		{
            get { return NormalizePath(m_basicConfiguration.eFlowInstallPath); }
		}

		private string eFlowOCRsPath
		{
			get { return NormalizePath(m_basicConfiguration.eFlowOCRsPath); }
		}

		private string eFlowTempPath
		{
			get { return NormalizePath(m_basicConfiguration.eFlowTempPath); }
		}
		
		private string NormalizePath(string sPath)
		{
			if(sPath.Length == 0 || sPath[sPath.Length-1]!=System.IO.Path.DirectorySeparatorChar)
			{
				return sPath;
			}

			return sPath.Substring(0, sPath.Length-1);
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (m_isWebSession == false)
            {
                var fullPath = System.IO.Path.Combine(
                    GetFullPathForSubPath(EFLOW_WORK_DIR),
                    ProcessUtil.UniqueProcessId);

                FileUtil.DeleteDirectory(fullPath, true);
            }
        }

        #endregion
    }
}
