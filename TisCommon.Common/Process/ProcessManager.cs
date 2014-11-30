using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Process
{
	public class ProcessManager
	{
		private string m_sBinPath;

		public ProcessManager(string sBinPath)
		{
			m_sBinPath = sBinPath;
		}

		public int StartProcess(
			string exeName, 
			string commandLine,
			bool   isInteractiveUserAccount = default(bool),
            string accountName = default(string),
			string password= default(string))
		{
            if (isInteractiveUserAccount)
			{
				ExceptionUtil.RaiseArgumentException(
					"bInteractiveUserAccount",
					"Not supported",
                    isInteractiveUserAccount,
					System.Reflection.MethodInfo.GetCurrentMethod());
			}

            if (exeName.IndexOfAny(new char[] { Path.PathSeparator }) >= 0)
			{
				throw new TisException(
					"Invalid sExe value, should contain EXE name without path");
			}

            string fullExePath = Path.Combine(m_sBinPath, exeName);

            if (!StringUtil.IsStringInitialized(accountName))
			{
                return StartProcessUnderCurrentAccount(fullExePath, commandLine);
			}

			return StartProcessUnderAccount(
				fullExePath,
                commandLine,
				accountName,
				password);
		}

		public void TerminateProcess(int nProcessId)
		{
            try
            {
                using (System.Diagnostics.Process oProcess = System.Diagnostics.Process.GetProcessById(nProcessId))
                {
                    oProcess.Kill();

                    Log.WriteInfo("ProcessManager.TerminateProcess: Process [{0}] is terminated.", nProcessId);
                }
            }
            catch(Exception exc)
            {
                Log.WriteError("ProcessManager.TerminateProcess: Failed to terminate process [{0}]. Details : [{1}]", 
                    nProcessId, exc.Message);
            }
		}


        private int StartProcessUnderAccount(
            string sExe,
            string sCmdLine,
            string sFullUser,
            string sPwd)
        {
            string[] UserNameParts = sFullUser.Split('\\');

            if (UserNameParts.Length != 2)
            {
                throw new TisException("Invalid user name [{0}], must be in [DOMAIN\\USER] format",
                    sFullUser);
            }

            string sDomain = UserNameParts[0];
            string sUser = UserNameParts[1];
            string currentDirectory = Path.GetDirectoryName(sExe);

            StringBuilder oCmdLine = new StringBuilder(sExe);

            if (StringUtil.IsStringInitialized(sCmdLine))
            {
                oCmdLine.Append(" ");
                oCmdLine.Append(sCmdLine);
            }

            PROCESS_INFORMATION oProcessInfo;
            STARTUPINFO oStartInfo;

            PrepareProcessInfo(out oProcessInfo, out oStartInfo);

            bool bResult = false;


            // Windows XP SP2 and Windows Server 2003: 
            // CreateProcessWithLogonW from a process that is running under the LocalSystem account will fail, 
            // because the function uses the logon SID in the caller token, 
            // and the token for the LocalSystem account does not contain this SID.

            try
            {
                oProcessInfo.dwProcessId = StartProcessAsUser(
                    sUser,
                    sDomain,
                    sPwd,
                    currentDirectory,
                    oCmdLine);


                bResult = oProcessInfo.dwProcessId > 0;
            }
            catch (Exception exc)
            {
                bResult = false;

                Log.WriteException(exc);
            }

            if (bResult == false)
            {
                try
                {
                    bResult = CreateProcessWithLogonW(
                        sUser,
                        sDomain,
                        sPwd,
                        0,  // dwFlags
                        null,
                        oCmdLine,
                        NORMAL_PRIORITY_CLASS | CREATE_UNICODE_ENVIRONMENT,
                        IntPtr.Zero,
                        currentDirectory,
                        ref oStartInfo,
                        ref	oProcessInfo);
                }
                catch (Exception oExc)
                {
                    Log.WriteException(oExc);
                }
            }

            if (bResult == false)
            {
                Log.WriteInfo("CreateProcessWithLogonW(User=[{0}], Domain=[{1}], Exe=[{2}]) failed, Error: [{3}]",
                        sUser,
                        sDomain,
                        sExe,
                        System.Runtime.InteropServices.Marshal.GetLastWin32Error()
                        );
            }
            else
            {
                Log.WriteInfo("Process (User=[{0}], Domain=[{1}], Exe=[{2}]) started.",
                    sUser,
                    sDomain,
                    oCmdLine.ToString()
                    );
            }

            return oProcessInfo.dwProcessId;
        }

        private int StartProcessAsUser(string sUser, string sDomain, string sPwd, string currentDirectory, StringBuilder oCmdLine)
		{
			bool bResult = false;

			IntPtr oUserToken = IntPtr.Zero;

			try
			{
				bResult = LogonUser(
					sUser,
					sDomain,
					sPwd,
					LOGON32_LOGON_INTERACTIVE,
					LOGON32_PROVIDER_DEFAULT,
					ref oUserToken);
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}

			if (bResult == false)
			{
				throw new TisException(
					"LogonUser(User=[{0}], Domain=[{1}], Exe=[{2}]) failed, Error: [{3}]",
					sUser,
					sDomain,
                    oCmdLine,
					System.Runtime.InteropServices.Marshal.GetLastWin32Error()
					);
			}

			PROCESS_INFORMATION oProcessInfo;
			STARTUPINFO         oStartInfo;

			PrepareProcessInfo(out oProcessInfo, out oStartInfo);

			try
			{
                bResult = CreateProcessAsUser(
					oUserToken,
					null,
                    oCmdLine,
					IntPtr.Zero,
					IntPtr.Zero,
					true,
					NORMAL_PRIORITY_CLASS | CREATE_UNICODE_ENVIRONMENT,
					IntPtr.Zero,
                    currentDirectory,
					ref oStartInfo,
					ref	oProcessInfo);
			}
			catch(Exception oExc)
			{
				Log.WriteException(oExc);
			}
		
			if(bResult == false)
			{
				throw new TisException(
					"CreateProcessAsUser(User=[{0}], Domain=[{1}], Exe=[{2}]) failed, Error: [{3}]",
					sUser,
					sDomain,
                    oCmdLine,
					System.Runtime.InteropServices.Marshal.GetLastWin32Error()
					);
			}

			return oProcessInfo.dwProcessId;
		}

		private int StartProcessUnderCurrentAccount(string sExe, string sCmdLine)
		{
            try
            {
                using (System.Diagnostics.Process oProcess = System.Diagnostics.Process.Start(
                          sExe,
                          sCmdLine))
                {
                    if (oProcess == null)
                    {
                        Log.WriteWarning("Failed to start process. Details : Exe [{0}], CmdLine [{1}]", sExe, sCmdLine);
                        return -1;
                    }

                    return oProcess.Id;
                }
            }
            catch(Exception exc)
            {
                Log.WriteException(exc);
                return -1;
            }
		}

		private void PrepareProcessInfo(out PROCESS_INFORMATION oProcessInfo, 
			                            out STARTUPINFO oStartInfo)
		{
			oProcessInfo = new PROCESS_INFORMATION();

			oStartInfo = new STARTUPINFO();

			oStartInfo.cb = Marshal.SizeOf(oStartInfo);
			oStartInfo.dwFlags = 0;

			oStartInfo.lpTitle = null;
		}

		#region P/Invoke

		//dwLogonFlags Specifies the logon option

		const int LOGON_WITH_PROFILE = 1;

		const int LOGON_NETCREDENTIALS_ONLY = 2;

		//dwCreationFlags - Specifies how the process is created

		const int CREATE_SUSPENDED = 0x00000004;

		const int CREATE_NEW_CONSOLE = 0x00000010;

		const int CREATE_NEW_PROCESS_GROUP = 0x00000200;

		const int CREATE_SEPARATE_WOW_VDM = 0x00000800;

		const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

		const int CREATE_DEFAULT_ERROR_MODE = 0x04000000;

		//dwCreationFlags parameter controls the new process's priority class

		const int NORMAL_PRIORITY_CLASS = 0x00000020;

		const int IDLE_PRIORITY_CLASS = 0x00000040;

		const int HIGH_PRIORITY_CLASS = 0x00000080;

		const int REALTIME_PRIORITY_CLASS = 0x00000100;

		const int BELOW_NORMAL_PRIORITY_CLASS = 0x00004000;

		const int ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000;

		//dwFlags

		// This is a bit field that determines whether certain STARTUPINFO

		// members are used when the process creates a window.

		// Any combination of the following values can be specified:

		const int STARTF_USESHOWWINDOW = 0x0000000;

		const int STARTF_USESIZE = 0x00000002;

		const int STARTF_USEPOSITION = 0x00000004;

		const int STARTF_USECOUNTCHARS = 0x00000008;

		const int STARTF_USEFILLATTRIBUTE = 0x00000010;

		const int STARTF_FORCEONFEEDBACK = 0x00000040;

		const int STARTF_FORCEOFFFEEDBACK = 0x00000080;

		const int STARTF_USESTDHANDLES = 0x00000100;

		const int STARTF_USEHOTKEY = 0x00000200;

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool CreateProcessWithLogonW(String lpszUsername,
			String lpszDomain, 
			String lpszPassword,
			int dwLogonFlags, 
			string applicationName, 
			StringBuilder commandLine,
			int creationFlags, 
			IntPtr environment,
			string currentDirectory,
			ref STARTUPINFO sui,
			ref PROCESS_INFORMATION processInfo);

		[StructLayout(LayoutKind.Sequential)]
		internal struct STARTUPINFO
		{
			internal int cb;

			[MarshalAs(UnmanagedType.LPTStr)]

			internal string lpReserved;

			[MarshalAs(UnmanagedType.LPTStr)]

			internal string lpDesktop;

			[MarshalAs(UnmanagedType.LPTStr)]

			internal string lpTitle;

			internal int dwX;

			internal int dwY;

			internal int dwXSize;

			internal int dwYSize;

			internal int dwXCountChars;

			internal int dwYCountChars;

			internal int dwFillAttribute;

			internal int dwFlags;

			internal short wShowWindow;

			internal short cbReserved2;

			internal IntPtr lpReserved2;

			internal IntPtr hStdInput;

			internal IntPtr hStdOutput;

			internal IntPtr hStdError;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct PROCESS_INFORMATION
		{
			internal IntPtr hProcess;

			internal IntPtr hThread;

			internal int dwProcessId;

			internal int dwThreadId;
		}

		const int LOGON32_LOGON_INTERACTIVE   = 2;
		const int LOGON32_LOGON_NETWORK       = 3;
		const int LOGON32_LOGON_BATCH         = 4;
		const int LOGON32_LOGON_SERVICE       = 5;

		const int LOGON32_PROVIDER_DEFAULT    = 0;

		[StructLayout(LayoutKind.Sequential)]
		internal struct SECURITY_ATTRIBUTES 
		{
			internal int Length;
			internal IntPtr lpSecurityDescriptor;
			internal bool bInheritHandle;
		}

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool LogonUser(
			string lpszUsername,
			string lpszDomain,
			string lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			ref IntPtr phToken
			);

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool CreateProcessAsUser(
			IntPtr hToken,
			string lpApplicationName,
			StringBuilder lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			bool bInheritHandles,
			int dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			ref STARTUPINFO lpStartupInfo,
			ref PROCESS_INFORMATION lpProcessInformation
			);		

		#endregion
	}
}
