using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Configuration
{
	[Guid("7E0F521C-34E7-4ce4-A993-3F02F37073BC")]
	public interface IConfigData
	{
		IGlobalConfigStorage MachineConfig { get; } 
	}

	/// <summary>
	/// This class is the factory for Config Storage
	/// </summary>
	[Guid("A6B79CCD-CF5E-46f3-B75D-F2F2EC864095")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class ConfigData: IConfigData
	{
		private static GlobalConfigStorage m_oMachineConfigStorage = null;

		public ConfigData()
		{

		}

		#region IConfigData

		public IGlobalConfigStorage MachineConfig 
		{
			get { return Machine; }
		}

		#endregion 

		public static IGlobalConfigStorage Machine 
		{ 
			get
			{
				return MachineConfigStorage;
			}				
		}

		public static IConfigStorage Process 
		{ 
			get { return null; }
		}

        public static object GetConfigurableData(string sectionName, Type configurableDataType)
        {
            object configurableData = null;

            IConfigSection configSection = ConfigData.Machine.GetSection(sectionName);

            if (configSection != null)
            {
                try
                {
                    configurableData = configSection.LoadFromXml();
                }
                catch (Exception exc)
                {
                    Log.WriteInfo(
                        "Failed loading contents of [{0}] section in configuration data, {1}",
                        sectionName,
                        exc.Message);
                }
            }

            if (configurableData == null)
            {
                try
                {
                    configurableData = Activator.CreateInstance(configurableDataType);
                }
                catch (Exception exc)
                {
                    Log.WriteInfo(
                        "Failed instantiating of configurable data type [{0}]. Details : [{1}]",
                        configurableDataType.Name,
                        exc.Message);

                    return null;
                }

                try
                {
                    if (configSection == null)
                    {
                        configSection = ConfigData.Machine.GetOrCreateSection(sectionName);
                    }

                    configSection.StoreAsXml(configurableData);

                    ConfigData.Machine.SaveAllSections();
                }
                catch (Exception exc)
                {
                    Log.WriteInfo(
                        "Failed saving contents of [{0}] section in configuration data, {1}",
                        sectionName,
                        exc.Message);
                }
            }

            return configurableData;
        }

        //
		//	Private
		//

		private static GlobalConfigStorage MachineConfigStorage
		{
			get
			{
				if (m_oMachineConfigStorage == null)
				{
					m_oMachineConfigStorage = new GlobalConfigStorage();
				}

				return m_oMachineConfigStorage;
			}
		}
	}
}
