using System;
using System.Threading;

namespace TiS.Core.TisCommon
{
	public class ConditionalLock: IDisposable
	{
		private bool   m_bLock;
		private object m_oObj;
        private int m_lockTryInterval = Timeout.Infinite;
        private bool m_isLocked;

        public ConditionalLock(object oObj, bool bLock)
            : this(oObj, bLock, Timeout.Infinite)
		{
		}

        public ConditionalLock(object oObj, bool bLock, int lockTryInterval)
        {
            m_oObj = oObj;
            m_bLock = bLock;
            m_lockTryInterval = lockTryInterval;

            if (m_bLock)
            {
                m_isLocked = Monitor.TryEnter(m_oObj, m_lockTryInterval);
            }
        }

        public bool IsLocked
        {
            get
            {
                return m_isLocked;
            }
        }

        public void Dispose()
		{
            if (m_bLock && m_isLocked)
			{
				Monitor.Exit(m_oObj);
			}
		}
	}


    #region ReaderWriterLock

    public abstract class RWLockSession : IDisposable
    {
        protected ReaderWriterLock m_locker;
        protected int m_timeoutInterval = Timeout.Infinite;

        public RWLockSession(ReaderWriterLock locker)
        {
            m_locker = locker;

            Lock();
        }

        public RWLockSession(ReaderWriterLock locker, int timeoutInterval)
            : this(locker)
        {
            m_timeoutInterval = timeoutInterval;
        }

        public abstract void Lock();

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }

    public class RWLockReadSession : RWLockSession
    {
        public RWLockReadSession(ReaderWriterLock locker)
            : base(locker)
        {
        }

        public RWLockReadSession(ReaderWriterLock locker, int timeoutInterval)
            : base(locker, timeoutInterval)
        {
        }

        public override void Lock()
        {
            m_locker.AcquireReaderLock(m_timeoutInterval);
        }

        #region IDisposable Members

        public override void Dispose()
        {
            m_locker.ReleaseReaderLock();
        }

        #endregion
    }

    public class RWLockWriteSession : RWLockSession
    {
        LockCookie m_cookie = new LockCookie();
        bool m_shouldRelease = false;

        public RWLockWriteSession(ReaderWriterLock locker)
            : base(locker)
        {
        }

        public RWLockWriteSession(ReaderWriterLock locker, int timeoutInterval)
            : base(locker, timeoutInterval)
        {
        }

        public override void Lock()
        {
            if (m_locker.IsReaderLockHeld)
            {
                m_cookie = m_locker.UpgradeToWriterLock(m_timeoutInterval);
            }
            else
            {
                m_locker.AcquireWriterLock(m_timeoutInterval);
                m_shouldRelease = true;
            }
        }

        #region IDisposable Members

        public override void Dispose()
        {
            if (m_locker.IsWriterLockHeld)
            {
                if (m_shouldRelease)
                {
                    m_locker.ReleaseWriterLock();
                }
                else
                {
                    m_locker.DowngradeFromWriterLock(ref m_cookie);
                }
            }
        }
        #endregion

    }

    #endregion
}
