using System;
using System.Threading;

namespace TiS.Core.TisCommon
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public class NonReentrantMutex
	{
		private bool m_bLocked = false;
		
		private const int LOCK_RETRY_INTERVAL = 100;

		//
		//	Public
		//

//		public bool Lock()
//		{
//			lock(this)
//			{
//				// Wait until a unit becomes available. We need to wait 
//				// in a loop in case someone else wakes up before us. This could 
//				// happen if the Monitor.Pulse statements were changed to Monitor.PulseAll 
//				// statements in order to introduce some randomness into the order
//				// in which threads are woken. 
//				while(m_bLocked)
//				{
//					if(Monitor.Wait(this) == false)
//					{
//						return false;
//					}
//				}
//
//				m_bLocked = true;
//
//				return true;
//			}
//		}

		public bool Lock(int nMilliseconds) 
		{ 
			DateTime oBegin = DateTime.Now; 

			while (GetMilliSecondsSince(oBegin) < nMilliseconds) 
			{
				if(TryLock())
				{
					return true;
				}

				Log.Write(
					Log.Severity.DETAILED_DEBUG,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					"Id=[{0}] Can't obtain lock, entering wait state",
					GetHashCode());

				Thread.Sleep(LOCK_RETRY_INTERVAL);
			}

			return false;
			
//			bool bLockObtained = false; 
//
//			try 
//			{ 
//				if ((bLockObtained = Monitor.TryEnter(this, nMilliseconds)) == true) 
//				{ 
//					// Wait until a unit becomes available. We need to wait 
//					// in a loop in case someone else wakes up before us. This could 
//					// happen if the Monitor.Pulse statements were changed to Monitor.PulseAll 
//					// statements in order to introduce some randomness into the order 
//					// in which threads are woken. 
//					while(m_bLocked) 
//					{ 
//						if (GetMilliSecondsSince(oBegin) > nMilliseconds) 
//						{
//							return false; 
//						}
//
//						if ((bLockObtained = Monitor.Wait(this, nMilliseconds)) == false) 
//						{
//							return false; 
//						}
//					} 
//
//					m_bLocked = true;
//
//					return true; 
//				} 
//				else 
//				{ 
//					return false; 
//				} 
//			} 
//			finally 
//			{ 
//				if (bLockObtained) 
//				{
//					Monitor.Exit(this); 
//				}
//			} 
		} 


		public void Unlock()
		{
			// Lock so we can work in peace. This works because lock is actually 
			// built around Monitor. 
			lock(this) 
			{ 
				if(!m_bLocked)
				{
					throw new TisException("Not locked");
				}

				// Release our hold on the unit of control. Then tell everyone 
				// waiting on this object that there is a unit available. 
				m_bLocked = false; 

//				Monitor.Pulse(this); 
			} 
		}

		public bool IsLocked
		{
			get 
			{ 
				lock(this)
				{
					return m_bLocked; 
				}
			}
		}

		//
		//	Private
		//
		
		private bool TryLock()
		{
			lock(this)
			{
				if(!m_bLocked)
				{
					m_bLocked = true;

					return true;
				}

				return false;
			}
		}

		private int GetMilliSecondsSince(DateTime oSince) 
		{ 
			TimeSpan t; 
			t = DateTime.Now - oSince; 
			return t.Seconds * 1000 + t.Minutes * 60000 + t.Hours * 3600000; 
		} 

	}
}
