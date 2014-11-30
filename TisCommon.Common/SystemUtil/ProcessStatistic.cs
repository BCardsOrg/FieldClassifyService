using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.SystemUtil
{
	[Serializable()]
	[DataContractAttribute(Namespace = "http://www.topimagesystems.com/eFlow")]
	public class ProcessStatisticData
	{
		/// <summary>
		/// Gets or sets the process id.
		/// </summary>
		/// <value>
		/// The process id.
		/// </value>
		[DataMember]
		public int ProcessId { get; set; }

		/// <summary>
		/// Gets or sets the name of the process.
		/// </summary>
		/// <value>
		/// The name of the process.
		/// </value>
		[DataMember]
		public string ProcessName { get; set; }

		/// <summary>
		/// Gets or sets the cpu usage (in 0-100%).
		/// </summary>
		/// <value>
		/// The cpu usage. [0-100%]
		/// </value>
		[DataMember]
		public int CpuUsage { get; set; }

		/// <summary>
		/// Gets or sets the memory (in Mb)
		/// </summary>
		/// <value>
		/// The memory [Mb]
		/// </value>
		[DataMember]
		public double Memory { get; set; }

		/// <summary>
		/// Gets or sets the IO throughput. (Read & Write)
		/// </summary>
		/// <value>
		/// The IO throughput. [Kb/sec]
		/// </value>
		[DataMember]
		public double IoThroughput { get; set; }
	}


	public static class ProcessStatistic
	{
		public static ProcessStatisticData GetStatistic(System.Diagnostics.Process process)
		{
			ProcessStatisticData result = new ProcessStatisticData()
			{
				ProcessId = process.Id,
				ProcessName = process.ProcessName
			};

			string name = GetPerformanceCounterProcessName(process);

			var cpuTask = Task.Factory.StartNew( () => result.CpuUsage = GetCpuUsgae(name) );
			var memTask = Task.Factory.StartNew(() => result.Memory = GetMemory(name));
			var ioTask = Task.Factory.StartNew(() => result.IoThroughput = GetIoThroughput(name));

			cpuTask.Wait();
			memTask.Wait();
			ioTask.Wait();

			cpuTask.Dispose();
			memTask.Dispose();
			ioTask.Dispose();

			return result;
		}

		public static int GetCpuUsgae(string name)
		{
			using (PerformanceCounter pcProcess = new PerformanceCounter("Process", "% Processor Time", name))
			{
				pcProcess.NextValue();
				System.Threading.Thread.Sleep(1000);
				// Return in [0-100%]
				return (int)(pcProcess.NextValue() + 0.5);
			}
		}

		public static double GetMemory(string name)
		{
			using (PerformanceCounter theMemCounter = new PerformanceCounter("Process", "Working Set", name))
			{
				// Return in [Mb]
				return (double)(theMemCounter.NextValue() / 1E+6);
			}
		}

		public static double GetIoThroughput(string name)
		{
			using (PerformanceCounter theHDCounter = new PerformanceCounter("Process", "IO Data Bytes/sec", name))
			{
				theHDCounter.NextValue();
				System.Threading.Thread.Sleep(1000);
				// Return in [Kb/sec]
				return (double)(theHDCounter.NextValue() / 1000); 
			}
		}


		public static string GetPerformanceCounterProcessName(System.Diagnostics.Process process)
		{
			int nameIndex = 1;
			string value = process.ProcessName;
			string counterName = process.ProcessName + "#" + nameIndex;
			PerformanceCounter pc = new PerformanceCounter("Process", "ID Process", counterName, true);

			while (true)
			{
				try
				{
					if (process.Id == (int)pc.NextValue())
					{
						value = counterName;
						break;
					}
					else
					{
						nameIndex++;
						counterName = process.ProcessName + "#" + nameIndex;
						pc = new PerformanceCounter("Process", "ID Process", counterName, true);
					}
				}
				catch (SystemException ex)
				{
					if (ex.Message == "Instance '" + counterName + "' does not exist in the specified Category.")
					{
						break;
					}
					else
					{
						throw;
					}
				}
			}

			return value;
		}
	}
}
