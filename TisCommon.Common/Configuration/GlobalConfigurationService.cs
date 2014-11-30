using System;
using System.Xml;
using System.Runtime.Serialization;
using Microsoft.Win32;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Security;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Configuration
{
    /// <summary>
    /// Base class used to access TiSConfiguration.xml file
    /// </summary>
    [Guid("63143A4D-A928-45F5-9BB8-68492CC9836F")]
    public interface IGlobalConfigurationService
    {
        object Load(string sectionName);

        void Save(object configObject, string sectionName);

        void RemoveSection(string sectionName);

        void RenameSection(string existingSectionName, string newSectionName);

        [Obsolete("Method is deprecated", true)]
        void SetConfigFilesPath(string sPathName);

        string GetConfigFilesPath();
    }


    [System.Runtime.InteropServices.ComVisible(false)]
    public class GlobalConfigurationService : ConfigurationService, IGlobalConfigurationService
    {
        // hold the last modified time of the config file
        private DateTime m_configLastWriteTime;

        //
        //	Public
        //

        public GlobalConfigurationService()
        {
        }


        #region IGlobalConfigurationStorage Members

        public object Load(string sectionName)
        {
            if (string.IsNullOrEmpty(CacheItemName) == true)
                CacheItemName = "GlobalConfigurationService";

            object configObject = base.Load(ConfigFileName, sectionName);

            return configObject;
        }

        public void Save(object configObject, string sectionName)
        {
            base.Save(configObject, ConfigFileName, sectionName);
        }

        public void RemoveSection(string sectionName)
        {
            base.RemoveSection(ConfigFileName, sectionName);
        }

        public void RenameSection(string existingSectionName, string newSectionName)
        {
            base.RenameSection(ConfigFileName, existingSectionName, newSectionName);
        }

        /// <summary>
        /// Update the path value for the config file  in the "ConfigFilesLocation" key in registry
        /// </summary>
        /// <param name="pathName"></param>
        [Obsolete("Method is deprecated", true)]
        public void SetConfigFilesPath(string pathName)
        {
        }

        public string GetConfigFilesPath()
        {
			return Path.GetDirectoryName(ProcessConfiguration.BasicConfigFile);
        }

        #endregion

        public bool ConfigFileExists
        {
            get
            {
                return File.Exists(ConfigFileName);
            }
        }

        protected bool NeedLoadData()
        {
            if (IsConfigFileChanged())
            {
                return true;
            }
            return false;
        }

        private string ConfigFileName
        {
            get
            {
                return ProcessConfiguration.BasicConfigFile;
            }
        }

		private void SaveConfigLastWriteTime()
        {
            if (!File.Exists(ConfigFileName))
                return;

            m_configLastWriteTime = File.GetLastWriteTime(ConfigFileName);
        }


        // check the last modified time , and
        // return if the config file was chenged or not
        private bool IsConfigFileChanged()
        {
            if (!File.Exists(ConfigFileName))
                return true;

            DateTime lastWriteTime = File.GetLastWriteTime(ConfigFileName);

            if (m_configLastWriteTime != lastWriteTime)
            {
                return true;
            }

            return false;
        }
    }
}
