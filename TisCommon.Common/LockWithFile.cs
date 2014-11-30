using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TiS.Core.TisCommon
{
	/// <summary>
	/// Lock session associate to a file
	/// </summary>
	public class LockWithFile : IDisposable
	{
		string m_lockFileName = string.Empty;

		FileStream m_lockFile = null;

		public LockWithFile(string lockFileName, int waitTimeInterval_msec = 100)
		{
			m_lockFileName = lockFileName;

			while (true)
			{
				try
				{
					m_lockFile = new FileStream(lockFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
					break;
				}
				// File is lock by other process, wait some time before try again
				catch 
				{
					System.Threading.Thread.Sleep(waitTimeInterval_msec);
				}
			}
		}

		public void Dispose()
		{
			if (m_lockFile != null)
			{
				m_lockFile.Dispose();
				m_lockFile = null;
				try
				{
					File.Delete(m_lockFileName);
				}
				// Maybe other process already lock it after  m_lockFile was Disposed - so the last lock session will delete the lock file
				catch
				{
				}
			}
		}
	}
}
