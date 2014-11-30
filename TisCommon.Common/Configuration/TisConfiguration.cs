using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Configuration
{

	[Guid("BC08B93F-F50E-4C02-BD8B-55890E27C032")]
	public interface ITisConfiguration
	{
		IConfigStorage	MachineConfig	{ get; }
	}


	/// <summary>
	/// TisMachineConfiguration .
	/// 
	/// this class expose the functionality for com clients
	/// to use the Configuration mechanism
	/// </summary>
	[Guid("1E565842-B273-4885-B600-F21043DFE951")]
	public class TisConfiguration : ITisConfiguration
	{
		public TisConfiguration()
		{

		}

		public IConfigStorage MachineConfig
		{
			get
			{
				return ConfigData.Machine;
			}
		}
	}
}
