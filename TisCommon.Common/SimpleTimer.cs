using System;
using System.Threading;

namespace TiS.Core.TisCommon
{
	public delegate void SimpleTimerCallback();
	
	public class SimpleTimer: IDisposable
	{
		Thread m_oThread = null;

		private SimpleTimerCallback m_pfCallback;

		private string m_sName;

        private long m_requestedInterval;
        private long m_lInterval;			

		private ManualResetEvent m_oEvent;

        private Delayer m_delayer;
        private bool m_IsStoped;

        private object m_locker = new object();

        //
		//	Public
		//

		public SimpleTimer(
			long				lIntervalMs,
			SimpleTimerCallback pfCallback,
			bool		        bStartImmediately,
			string				sName
			)
		{
            m_requestedInterval = lIntervalMs;
			m_lInterval  = lIntervalMs;
			m_pfCallback = pfCallback;
			m_sName      = sName;

			m_oEvent = new ManualResetEvent(false);

            m_delayer = new Delayer(m_locker);

			if(bStartImmediately)
			{
				Start();
			}
		}

		public SimpleTimer(
			long lIntervalMs,
			SimpleTimerCallback pfCallback,
			string				sName)
			:this(lIntervalMs, pfCallback, false, sName)
		{
		}

		public SimpleTimer(
			TimeSpan			oInterval,
			SimpleTimerCallback pfCallback,
			bool		        bStartImmediately,
			string				sName
			)
			:this((long)oInterval.TotalMilliseconds, pfCallback, bStartImmediately, sName)
		{
		}

		public void Start()
		{
            Start(ApartmentState.MTA);
		}

        public void Start(ApartmentState apartmentState)
        {
            m_IsStoped = false;

            lock (m_locker)
            {
                if (m_oThread == null)
                {
                    m_oThread = new Thread(new ThreadStart(TimerProc));
                    m_oThread.TrySetApartmentState(apartmentState);
                    m_oThread.IsBackground = true;
                    m_oThread.Name = m_sName;
                    m_oThread.Start();
                }
            }
        }

		public void Stop()
		{
            m_IsStoped = true;

            lock (m_locker)
			{
				if(m_oThread != null)
				{
                    m_oThread.Join();
                    m_oThread = null;
				}

                m_oEvent = null;
			}
		}

        public void Dispose()
		{
            m_IsStoped = true;

            TriggerOnceNow();

			Stop();
		}

        public void TriggerOnceNow()
		{
            lock (m_locker)
			{
				m_oEvent.Set();
			}
		}

        public void TriggerOnceDelayed(int delayInMseconds)
        {
            m_delayer.Trigger(delayInMseconds);

            TriggerOnceNow();
        }

        public void TriggerOnceDelayed(TimeSpan delayTime)
        {
            TriggerOnceDelayed(delayTime.Milliseconds);
        }

        public void Suspend()
        {
            lock (m_locker)
            {
                m_lInterval = -1;
            }
        }

        public void Resume()
        {
            lock (m_locker)
            {
                m_lInterval = m_requestedInterval;
            }
        }

        public static void InitTimer(
			ref SimpleTimer	    oTimer,
			long				lIntervalMs,
			SimpleTimerCallback pfCallback,
			bool		        bStartImmediately,
			string				sName)
		{
			if(oTimer != null)
			{
				ExceptionUtil.RaiseInvalidOperationException(
					System.Reflection.MethodInfo.GetCurrentMethod(),
					"Can't start timer " + sName + ", already created");
			}

			oTimer = new SimpleTimer(
				lIntervalMs,
				pfCallback,
				bStartImmediately,
				sName
				);
		}

		public static void DisposeTimer(
			ref SimpleTimer	oTimer)
		{
			if(oTimer != null)
			{	
				// Dispose timer
				oTimer.Dispose();
				
				// Set to null
				oTimer = null;
			}
		}

		//
		//	Private
		//

		private void TimerProc()
		{
            while (true)
			{
				try
				{
                    if (m_IsStoped) { return; }
                    m_delayer.Delay();
                    if (m_IsStoped) { return; }

					m_oEvent.Reset();

					try
					{
						// Call callback
						m_pfCallback();
					}
					catch(Exception oExc)
					{
						Log.Write(
							Log.Severity.DEBUG,
							"Failed calling delegate [{0}], {1}",
							m_pfCallback,
							oExc.Message);
					}
					
					m_oEvent.WaitOne((int)m_lInterval, false);
				}
				catch(ThreadAbortException exc)
				{
					Log.WriteException(Log.Severity.INFO, exc, "Timer [{0}] stopped", m_sName);
					return;
				}
				catch(Exception oExc)
				{
					Log.WriteException(
						Log.Severity.WARNING, 
						oExc);
				}
			}
		}

        private class Delayer
        {
            private bool m_isTriggered;
            private int m_delayInMseconds;
            private object m_locker;

            public Delayer(object locker)
            {
                m_locker = locker;
            }

            public void Trigger(int delayInMseconds)
            {
                lock (m_locker)
                {
                    m_delayInMseconds = delayInMseconds;
                    m_isTriggered = true;
                }
            }

            public void Delay()
            {
                lock (m_locker)
                {
                    if (m_isTriggered)
                    {
                        Thread.Sleep(m_delayInMseconds);

                        m_isTriggered = false;
                    }
                }
            }
        }
	}
}
