using System;
using System.IO;

namespace TiS.Core.TisCommon.Configuration
{
	[System.Runtime.InteropServices.Guid("2D310F2D-553B-4731-94E1-A7B38807A360")]
	public interface IGlobalConfigStorage: IConfigStorage
	{
        /// <summary>
        /// Obsolete :sets the data path.
        /// </summary>
        /// <param name="sPathName">Name of the s path.</param>
        [Obsolete("Method is deprecated", true)]
        void SetDataPath(string sPathName);
		string GetDataPath();
	}

	[System.Runtime.InteropServices.ComVisible(false)]
	public class GlobalConfigStorage: BaseConfigStorage, IGlobalConfigStorage
	{
        // hold the last modified time of the config file
		private DateTime	m_configLastWriteTime;
		private static NamedMutex m_oConfigStorageMutex = new NamedMutex(false, @"Global\ConfigStorageFSMutex");


		//
		//	Public
		//

		public GlobalConfigStorage()
		{
		}

		/// <summary>
		/// Obsolete : update the path value for the config file  in the "ConfigFilesLocation" key in registry
		/// </summary>
		/// <param name="sPathName"></param>
        [Obsolete("Method is deprecated", true)]
		public void SetDataPath(string sPathName)
		{
		}

		public string GetDataPath()
		{
            return Path.GetDirectoryName(ProcessConfiguration.BasicConfigFile);
		}



		//
		//	Protected
		//

		protected override string LoadConfigXML()
		{
			using(AutoMutexLock oMutex = new AutoMutexLock(m_oConfigStorageMutex) )
			{
				string sConfigXML = String.Empty;

				if(IsConfigFileExists())
				{
					sConfigXML = FileUtil.GetFileAsString(ConfigFileName);
				}

				SaveConfigLastWriteTime();

				return sConfigXML;
			}
		}
 
		protected override void StoreConfigXML(string sXML)
		{
			using(AutoMutexLock oMutex = new AutoMutexLock(m_oConfigStorageMutex) )
			{
				string sConfigFileName = ConfigFileName;
				
				string sDir = Path.GetDirectoryName(sConfigFileName);

				PathUtil.EnsurePathExist(sDir);

				FileUtil.CreateFileFromString(
					ConfigFileName,
					sXML);

				SaveConfigLastWriteTime();
			}
		}

		protected override bool NeedLoadData()
		{
			if(IsConfigFileChanged())
			{
				return true;
			}

			return base.NeedLoadData ();
		}

		//
		//	Private
		//

		private string ConfigFileName
		{			
			get
			{
                return ProcessConfiguration.BasicConfigFile;
			}
		}

		private void SaveConfigLastWriteTime()
		{
			if (!IsConfigFileExists()) 
				return;

			m_configLastWriteTime = File.GetLastWriteTime(ConfigFileName);
		}

		private bool IsConfigFileExists() 
		{
			return File.Exists(ConfigFileName);
		}

		// check the last modified time , and
		// return if the config file was chenged or not
		private bool IsConfigFileChanged()
		{
			if (!IsConfigFileExists()) 
				return true;
			
			DateTime lastWriteTime = 
				File.GetLastWriteTime(ConfigFileName);

			if ( m_configLastWriteTime !=  lastWriteTime)
			{
				return true;
			}

			return false;
		}
    }
}
