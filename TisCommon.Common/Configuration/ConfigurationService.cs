using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Configuration;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Cache;
using System.Collections.Concurrent;

namespace TiS.Core.TisCommon.Configuration
{
    [ComVisible(false)]
    public class ConfigurationService
    {
        /// <summary>
        /// The setup dir
        /// </summary>
        protected const string SETUP_DIR = "Setup";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService"/> class.
        /// </summary>
        public ConfigurationService()
        {
        }

        /// <summary>
        /// Gets or sets the name of the cache item.
        /// </summary>
        /// <value>
        /// The name of the cache item.
        /// </value>
        protected string CacheItemName { get; set; }

        /// <summary>
        /// Loads the specified configuration file name.
        /// </summary>
        /// <param name="configFileName">Name of the configuration file.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public virtual object Load(string configFileName, string sectionName)
        {
            object configObject = null;

			IDataCache localCache = CacheFactory.LocalCache;

			// Get it form cache...
			ConcurrentDictionary<string, object> sectionNameToConfigObjectMap = (ConcurrentDictionary<string, object>)localCache.GetOrAdd(CacheItemName, k => new ConcurrentDictionary<string, object>());

            string internalCacheItem = GetInternalCacheItemName(configFileName, sectionName);

            if (!sectionNameToConfigObjectMap.TryGetValue(internalCacheItem, out configObject))
            {
				// Load from storage 
                configObject = LoadSection(configFileName, sectionName);

                if (configObject != null)
                {
					sectionNameToConfigObjectMap.AddOrUpdate(internalCacheItem, configObject, (key, oldValue) => configObject);
                }
            }


            return configObject;
        }

        /// <summary>
        /// Saves the specified configuration object.
        /// </summary>
        /// <param name="configObject">The configuration object.</param>
        /// <param name="configFileName">Name of the configuration file.</param>
        /// <param name="sectionName">Name of the section.</param>
        public virtual void Save(object configObject, string configFileName, string sectionName)
        {
            if (configObject != null)
            {
				// Save to storage
				SaveSection(configObject, configFileName, sectionName);

				// Update cache...
				ConcurrentDictionary<string, object> sectionNameToConfigObjectMap = (ConcurrentDictionary<string, object>)CacheFactory.LocalCache.GetOrAdd(CacheItemName, k => new ConcurrentDictionary<string, object>());

                string internalCacheItem = GetInternalCacheItemName(configFileName, sectionName);

				sectionNameToConfigObjectMap.AddOrUpdate(internalCacheItem, configObject, (key, oldValue) => configObject);

				// This PutCacheItem(), will cause other IIS to get the changes
				CacheFactory.LocalCache.PutCacheItem(CacheItemName, sectionNameToConfigObjectMap);
            }
        }

        /// <summary>
        /// Removes the section.
        /// </summary>
        /// <param name="configFileName">Name of the configuration file.</param>
        /// <param name="sectionName">Name of the section.</param>
        public virtual void RemoveSection(string configFileName, string sectionName)
        {
            System.Configuration.Configuration config = OpenExeConfiguration(configFileName, false);

            if (config == null)
            {

                Log.Write(Log.Severity.WARNING,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "Cannot load <{0}> configuration. ", configFileName);

                return;
            }

            try
            {
                using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
                {
                    CustomSection currentSection = (CustomSection)config.Sections[sectionName];

                    if (currentSection == null)
                    {
                        Log.Write(Log.Severity.INFO,
                            System.Reflection.MethodInfo.GetCurrentMethod(),
                            " No relevant section in the local configuration file found. Default station options will be used.");

                        return;
                    }
                    else
                    {
                        config.Sections.Remove(sectionName);
                    }
                }
            }
            catch (ConfigurationException exc)
            {
                Log.WriteException(exc);
            }
        }

        /// <summary>
        /// Renames the section.
        /// </summary>
        /// <param name="configFileName">Name of the configuration file.</param>
        /// <param name="existingSectionName">Name of the existing section.</param>
        /// <param name="newSectionName">New name of the section.</param>
        public virtual void RenameSection(string configFileName, string existingSectionName, string newSectionName)
        {
          
            System.Configuration.Configuration config = OpenExeConfiguration(configFileName, false);

            if (config == null)
            {

                Log.Write(Log.Severity.WARNING,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "Cannot load <{0}> configuration. ", configFileName);

                return;
            }
            try
            {
                using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
                {
                    CustomSection currentSection = (CustomSection)config.Sections[existingSectionName];

                    if (currentSection == null)
                    {
                        Log.Write(Log.Severity.INFO,
                            System.Reflection.MethodInfo.GetCurrentMethod(),
                            " No relevant section in the local configuration file found. Default station options will be used.");

                        return;
                    }
                    else
                    {
                        config.Sections.Remove(existingSectionName);

                        config.Sections.Add(newSectionName, currentSection);
                    }
                }
            }
            catch (ConfigurationException exc)
            {
                Log.WriteException(exc);
            }
        }

        #region Configuration sections

        /// <summary>
        /// Save configuration to specified configuration file
        /// </summary>
        /// <param name="configObject">Configuration object to be saved</param>
        /// <param name="configFileName">Configuration file name</param>
        /// <param name="sectionName">Configuration section name</param>
        protected void SaveSection(object configObject, string configFileName, string sectionName)
        {
            try
            {
                System.Configuration.Configuration config = OpenExeConfiguration(configFileName, true);

                CustomSection currentSection = (CustomSection)config.Sections[sectionName];

                if (currentSection == null)
                {
                    config.Sections.Add(sectionName, new CustomSection());
                }

                currentSection = (CustomSection)config.Sections[sectionName];

                currentSection.Data = configObject;

                currentSection.SectionInformation.ForceSave = true;

                config.Save(ConfigurationSaveMode.Minimal);

            }
            catch (ConfigurationErrorsException exc)
            {
                Log.Write(Log.Severity.ERROR,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    " Error : {0}", exc.Message);

                throw;
            }

        }

        /// <summary>
        /// Load configuration object from specified configuration file
        /// </summary>
        /// <param name="configFileName">Configuration file name</param>
        /// <param name="sectionName">Configuration section name</param>
        /// <returns>
        /// Configuration object
        /// </returns>
        protected object LoadSection(string configFileName, string sectionName)
        {
           System.Configuration.Configuration config = OpenExeConfiguration(configFileName, false);

            if (config == null)
            {

                Log.Write(Log.Severity.INFO,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    " No local configuration file found. Default station options will be used.");

                return null;
            }

            CustomSection currentSection;

            using (AssemblyVersionIgnorer versionResolver = new AssemblyVersionIgnorer())
            {
                currentSection = (CustomSection)config.Sections[sectionName];
            }

            if (currentSection == null)
            {
                Log.Write(Log.Severity.INFO,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    " No relevant section in the local configuration file found. Default station options will be used.");

                return null;
            }

            object configObject = currentSection.Data;

            return configObject;

        }

        #region Private methods

        /// <summary>
        /// Opens the executable configuration.
        /// </summary>
        /// <param name="configFileName">Name of the configuration file.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns></returns>
        private System.Configuration.Configuration OpenExeConfiguration(string configFileName, bool create)
        {
            System.Configuration.Configuration config;

            try
            {
                if (!System.IO.File.Exists(configFileName) && (create == false))
                {
                    Log.Write(Log.Severity.INFO,
                        "Local configuration file <{0}> not found. Default values will be used.",
                        configFileName);

                    return null;
                }

                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

                fileMap.ExeConfigFilename = configFileName;

                config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                return config;

            }
            catch (Exception exc)
            {
                Log.Write(Log.Severity.ERROR,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    " Error : {0}", exc.Message);

                return null;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets the name of the internal cache item.
        /// </summary>
        /// <param name="configFileName">Name of the configuration file.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        private string GetInternalCacheItemName(string configFileName, string sectionName)
        {
            return configFileName + "_" + sectionName;
        }

    }
}
