using System;
using Microsoft.Win32;


namespace TiS.Core.TisCommon
{
	/// <summary>
	/// AutoMutexLock.
	/// 
	/// This object is wrapper for system mutex
	/// and the lock is done automatically , and the mutex released
	/// by it's dispose method 
	/// </summary>
	public class AutoMutexLock : IDisposable
	{
		// time out for mutex locking is 20 seconds
		const int	MUTEX_TIMEOUT = 20000;
		
		// indicate if the mutex is locked or not
		private bool m_bLock = false;

		private NamedMutex m_Mutex = null;

		/// <summary>
		/// This object lock system mutex 
		/// </summary>
		/// <param name="nLockTimeOut">The time out in milliseconds to try the locking</param>
		public AutoMutexLock(NamedMutex oMutex,int nLockTimeOut)
		{
			m_Mutex = oMutex;
			DoMutexLocking(nLockTimeOut);
		}

		/// <summary>
		/// This object lock system mutex , The default timeout locking : 20 seconds
		/// </summary>
		public AutoMutexLock(NamedMutex oMutex)
		{
			m_Mutex = oMutex;
			DoMutexLocking(MUTEX_TIMEOUT);
		}

        public bool IsLocked
        {
            get { return m_bLock; }
        }

		private void DoMutexLocking(int nLockTimeOut)
		{
			if (m_Mutex != null)
			{
				m_bLock = m_Mutex.WaitOne(nLockTimeOut,false);
			}

			if (false == m_bLock)
			{
				// Throw exception incase the locking was failed
				throw new Exception("AutoMutexLock.DoMutexLocking failed to lock the AUTO_MUTEX_LOCK");
			}		
		}

		public void Dispose()
		{
			if(m_bLock)
			{
				m_bLock = false;

                if (m_Mutex != null)
                {
                    while (true)
                    {
                        if (m_Mutex.ReleaseMutex() == false)
                            break;
                    }
                }
			}
		}
	}
}
