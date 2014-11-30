using System;
using System.Collections.Generic;

namespace TiS.Core.TisCommon
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public class MachineInfo
	{
        private static List<string> m_oMachineNameAliases = new List<string>();
		private static string       m_sPrimaryAlias;

		static MachineInfo()
		{
			m_oMachineNameAliases.Add(Environment.MachineName);
			m_oMachineNameAliases.Add("127.0.0.1");
			m_oMachineNameAliases.Add("localhost");
			m_oMachineNameAliases.Add("(local)");
		}

		public static string[] LocalMachineAliases
		{
			get
			{
				lock(typeof(MachineInfo))
				{
                    return m_oMachineNameAliases.ToArray();  
				}
			}
		}
		
		public static void AddMachineNameAlias(
			string  sAlias,
			bool	bPrimary)
		{
			lock(typeof(MachineInfo))
			{
				if(!IsLocalMachineName(sAlias))
				{
					m_oMachineNameAliases.Add(sAlias);
				}
			
				if(bPrimary)
				{
					m_sPrimaryAlias = sAlias;
				}
			}
		}

		public static string MachineName
		{
			get
			{
				lock(typeof(MachineInfo))
				{
					if(StringUtil.IsStringInitialized(m_sPrimaryAlias))
					{
						return m_sPrimaryAlias;
					}

					return Environment.MachineName;
				}
			}
		}

		public static bool IsLocalMachineName(string sMachineName)
		{
			lock(typeof(MachineInfo))
			{
				foreach(string sAlias in m_oMachineNameAliases)
				{
					if(StringUtil.CompareIgnoreCase(sAlias, sMachineName))
					{
						return true;
					}
				}
			}

			return false;
		}

	}
}
