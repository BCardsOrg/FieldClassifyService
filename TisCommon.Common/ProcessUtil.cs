using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon
{
   // [CLSCompliant(false)]
	public class ProcessUtil
	{
		[DllImport("kernel32.dll", SetLastError=true)] 
		static extern int GetCurrentProcessId ();

		private static string m_sUniqueProcessId;
		private static int    m_nProcessId = 0;
		private static string m_sProcessName;
		private static string m_sProcessMainModule;

		public const string eFlowAutorunStationService = "eFlowAutorunStationStarter";


		//
		//	Public
		//	

		public static int CurrentProcessId
		{
			get
			{
				InitIfRequired();

				return m_nProcessId;
			}
		}

		public static string CurrentProcessName
		{
			get
			{
				InitIfRequired();

				return m_sProcessName;
			}
		}

		public static string CurrentProcessMainModule
		{
			get
			{
				InitIfRequired();

				return m_sProcessMainModule;
			}
		}

		public static string UniqueProcessId
		{
			get
			{
				lock(typeof(ProcessUtil))
				{
					if(!StringUtil.IsStringInitialized(m_sUniqueProcessId))
					{
						m_sUniqueProcessId   = CreateUniqueProcessId();
					}

					return m_sUniqueProcessId;
				}
			}
		}

		//
		//	Private
		//
		
		private static bool IsInitialized
		{
			get 
			{
				return m_nProcessId > 0;
			}
		}

		private static void InitIfRequired()
		{
			lock(typeof(ProcessUtil))
			{
				if(!IsInitialized)
				{
					try
					{
						m_nProcessId         = GetCurrentProcessId();
						m_sProcessMainModule = System.Windows.Forms.Application.ExecutablePath;
						m_sProcessName       = Path.GetFileName(m_sProcessMainModule);
					}
					catch(Exception oExc)
					{
						Log.WriteException(oExc);

						throw;
					}
				}
			}
		}

		private static string CreateUniqueProcessId()
		{
			// Get process id
			int nProcessId = CurrentProcessId;
			
			// Get machine name
            string sMachineName = MachineInfo.MachineName;

			// Get current exe name
			string sExeName = Path.GetFileNameWithoutExtension(PathUtil.GetExeName());
	
			// Create a random 64-bit number
			ulong lInstanceNumber = RandomUtil.GetUInt64();

			string sUniqueProcessId = String.Format(
				"{0}_{1}_{2}_{3}", 
				sMachineName,
				sExeName,
				nProcessId,
				lInstanceNumber);

			return sUniqueProcessId;
		}

        public static void TerminateCurrentProcess(string errorStringFormat, params object[] args)
        {
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

            Log.WriteFatalError(
                String.Format(errorStringFormat, args) +
                String.Format("Process [{0}] will be terminated.", currentProcess.ProcessName));

            currentProcess.Kill();
        }
    }
}
