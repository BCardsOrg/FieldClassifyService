using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;

namespace TiS.Core.TisCommon
{
	sealed public class NamedMutex : WaitHandle
	{
		public NamedMutex(bool bInitialOwner, string strName)
		{
			InternalCreateMutex(bInitialOwner, strName);
		}

		public string MutexName
		{
			get{return m_strMutexName;}
		}

		private string InternalMutexName
		{
			get{return m_strMutexName;}
			set
			{
				if(value == null)
				{
					throw new ArgumentNullException("strName", "Parameter strName cannot be null");
				}
				m_strMutexName = value;
			}
		}

		private void InternalCreateMutex(bool bInitialOwner, string strName)
		{
			try
			{
				lock(padlock)
				{
					InternalMutexName = strName;
					SECURITY_DESC sd;
					SECURITY_ATT sa = new SECURITY_ATT();

					SecurityDescriptor.GetNullDaclSecurityDescriptor(out sd);
					sa.lpSecurityDescriptor = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SECURITY_DESC)));
					Marshal.StructureToPtr(sd, sa.lpSecurityDescriptor, false);        
					sa.bInheritHandle = false;
					sa.nLength = Marshal.SizeOf(typeof(SECURITY_ATT));

                    Handle = CreateMutexW(ref sa, bInitialOwner, InternalMutexName);

                    if (Handle == InvalidHandle)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    //this.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(handle, true);

				}
			}
			catch{throw;}
		}

		public bool ReleaseMutex()
		{
			return InternalReleaseMutex();
		}

        private bool InternalReleaseMutex()
        {
            bool res = ReleaseMutex(Handle);
           
            return res;
        }

		[DllImport("Kernel32.dll", SetLastError=true, CharSet=CharSet.Unicode)]
		private static extern IntPtr CreateMutexW(ref SECURITY_ATT sd, bool bInitialOwner, string strName);

		[DllImport("Kernel32.dll", SetLastError=true)]
		private static extern bool ReleaseMutex(IntPtr hMutex);

		private object padlock = new object();
		private string m_strMutexName = null;
	}

	internal class SecurityDescriptor
	{
		internal static void GetNullDaclSecurityDescriptor(out SECURITY_DESC sd)
		{
			if(InitializeSecurityDescriptor(out sd, 1))
			{
				if(!SetSecurityDescriptorDacl(ref sd, true, IntPtr.Zero, false))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
			else
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		[DllImport("Advapi32.dll", SetLastError=true)]
		private static extern bool SetSecurityDescriptorDacl(ref SECURITY_DESC sd, bool bDaclPresent, IntPtr Dacl, bool bDaclDefaulted);

		[DllImport("Advapi32.dll", SetLastError=true)]
		private static extern bool InitializeSecurityDescriptor(out SECURITY_DESC sd, int dwRevision);
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct SECURITY_DESC
	{
		private byte Revision;
		private byte Sbz1;
		private ushort Control;
		private IntPtr Owner;
		private IntPtr Group;
		private IntPtr Sacl;
		private IntPtr Dacl;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct SECURITY_ATT
	{
		public int nLength;
		public IntPtr lpSecurityDescriptor;
		public bool bInheritHandle;
	}
}
