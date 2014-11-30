using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Management;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.SystemUtil
{
	[Serializable()]
	[DataContractAttribute(Namespace = "http://www.topimagesystems.com/eFlow")]
	public class ServerStatisticData
	{
		/// <summary>
		/// Gets or sets the cpu usage (in 0-100%).
		/// </summary>
		/// <value>
		/// The cpu usage. [0-100%]
		/// </value>
		[DataMember]
		public int CpuUsage { get; set; } 

		/// <summary>
		/// Gets or sets the mem usage (in 0-100%).
		/// </summary>
		/// <value>
		/// The mem usage. [0-100%]
		/// </value>
		[DataMember]
		public int MemUsage { get; set; }

		/// <summary>
		/// Gets or sets the hd free space (in 0-100%)
		/// </summary>
		/// <value>
		/// The hd free space. [0-100%]
		/// </value>
		[DataMember]
		public int HdFreeSpace { get; set; }
	}

	public static class ServerStatistic
	{
		public static ServerStatisticData GetStatistic(string mainHdName)
		{
			var result = new ServerStatisticData();

			// Get CPU usage
			result.CpuUsage = GetCpuUsage();

			// Get Memory usage
			result.MemUsage = GetMemUsage();

			// Get HD free space
			result.HdFreeSpace = GetHdFreeSpace(mainHdName);

			return result;
		}

		public static int GetHdFreeSpace( string hdName )
		{
			var driveInfo = DriveInfo.GetDrives().Where(d => string.Compare(d.Name, hdName, true) == 0).FirstOrDefault();
			if (driveInfo == null)
				throw new TisException("Hard disk name not exist: '{0}'", hdName);
			return (int)((driveInfo.TotalFreeSpace * 100 / driveInfo.TotalSize));
		}

		public static int GetCpuUsage()
		{
			using (PerformanceCounter pcProcess = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
			{
				pcProcess.NextValue();
				Thread.Sleep(1000);
				return (int)(pcProcess.NextValue() + 0.5);
			}
		}

		public static int GetMemUsage()
		{
			double memAvailable, memPhysical;

			using (PerformanceCounter pCntr = new PerformanceCounter("Memory", "Available KBytes"))
			{
				memAvailable = (double)pCntr.NextValue() * 1000;
			}

			memPhysical = GetPhysicalMemory();

			return (int)((memPhysical - memAvailable) * 100 / memPhysical);
		}

		static private double GetPhysicalMemory()
		{
			ManagementObjectSearcher searcher =
				new ManagementObjectSearcher(string.Format("select * from Win32_PhysicalMemory"));

			double result = 0;
			foreach (ManagementObject xxx in searcher.Get())
			{
				result += double.Parse(xxx.Properties["Capacity"].Value.ToString());
			}

			return result;
		}
	}
}
