using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace TiS.Core.TisCommon
{
	public delegate void VoidNoParamsDelegate();

    #region ParallelCall

    [System.Runtime.InteropServices.ComVisible(false)]
	public class ParallelCall
	{
		private ArrayList m_oCallEntries = new ArrayList(); 

		private int m_nThreadsToUse = 5;

		public ParallelCall()
		{
		}
		
		public int ThreadsToUse
		{
			get
			{
				return m_nThreadsToUse;
			}
			set
			{
				m_nThreadsToUse = value;
			}
		}

		public void Add(Delegate oDelegate)
		{
			Add(oDelegate, EmptyArrays.ObjectArray);
		}

		public void Add(Delegate pfDelegate, object[] Params)
		{
			CallEntry oCallEntry = new CallEntry(pfDelegate, Params);

			m_oCallEntries.Add(oCallEntry);
		}

        public object[] Perform()
        {
            return Perform(false);
        }

        public object[] Perform(bool needResults)
        {
            return Perform(null, needResults);
        }

        public object[] Perform(AsyncCallback callBack, bool needResults)
		{
			int nCurrCallIndex = 0;

            List<object> finalResults = new List<object>();

            object[] Results;

            while (nCurrCallIndex < m_oCallEntries.Count)
			{
				int nThreads = GetThreadsToUse();

				if(nThreads > 1)
				{
					// Invoke asynchronously
					int nCount = Math.Min(
						nThreads, 
						m_oCallEntries.Count-nCurrCallIndex);

                    Results = Perform(nCurrCallIndex, nCount, callBack, needResults);
			
					nCurrCallIndex += nCount;
				}
				else
				{
					// Invoke synchronously
					CallEntry oCall = (CallEntry)m_oCallEntries[nCurrCallIndex];

                    Results = new object[] { oCall.Invoke() };

					nCurrCallIndex++;
				}

                foreach(object result in Results)
                {
                    finalResults.Add(result);
                }
			}

            return finalResults.ToArray();
		}

		//
		//	Private
		//

		private int GetThreadsToUse()
		{
			//int nAvailableThreads = GetAvailableWorkerThreads();
	
			//eturn nAvailableThreads / 2;

			return ThreadsToUse;
		}

		private int GetAvailableWorkerThreads()
		{
			int nWorkerThreads, nDummy;
			
			System.Threading.ThreadPool.GetAvailableThreads(
				out nWorkerThreads, out nDummy);

			return nWorkerThreads;
		}

		private object[] Perform(
			int nCallStartIndex,
            int nCount,
            AsyncCallback callBack,
            bool needResults)
		{
            object[] Results = new object[nCount];
            IAsyncResult[] ars = new AsyncResult[nCount];

            for (int i = 0; i < nCount; i++)
			{	
				CallEntry oCall = 
					(CallEntry)m_oCallEntries[i+nCallStartIndex];

                ars[i] = oCall.BeginInvoke(callBack, oCall);
			}

            if (callBack == null && needResults)
            {
                for (int i = 0; i < nCount; i++)
                {
                    CallEntry oCall =
                        (CallEntry)m_oCallEntries[i + nCallStartIndex];

                    Results[i] = oCall.EndInvoke(ars[i]);
                }
            }

            return Results;
		}
    }

    #endregion

    #region CallEntry

    public class CallEntry
    {
        private const int CALL_TIMEOUT = 60 * 1000;

        private const string BEGIN_INVOKE_METHOD = "BeginInvoke";
        private const string END_INVOKE_METHOD = "EndInvoke";

        object[] m_Params;

        Delegate m_pfDelegate;

        public CallEntry(Delegate pfDelegate, object[] Params)
        {
            m_pfDelegate = pfDelegate;
            m_Params = Params;
        }

        public object[] Params
        {
            get { return m_Params; }
        }

        public object Invoke()
        {
            return m_pfDelegate.DynamicInvoke(m_Params);
        }

        public IAsyncResult BeginInvoke(AsyncCallback callBack, object userObject)
        {
            return BeginInvoke(m_pfDelegate, m_Params, callBack, userObject);
        }

        public object EndInvoke(IAsyncResult ar)
        {
            return EndInvoke(m_pfDelegate, ar);
        }

        //
        //	Private
        //

        private static IAsyncResult BeginInvoke(
            Delegate pfDelegate,
            object[] Params,
            AsyncCallback callBack,
            object userObject)
        {
            MethodInfo oBeginInvokeMethod =
                GetMethod(pfDelegate, BEGIN_INVOKE_METHOD);

            return (IAsyncResult)oBeginInvokeMethod.Invoke(
                pfDelegate,
                BuildAsyncParams(Params, callBack, userObject));
        }

        private static object EndInvoke(Delegate pfDelegate, IAsyncResult oResult)
        {
            MethodInfo oEndInvokeMethod =
                GetMethod(pfDelegate, END_INVOKE_METHOD);

            if (oResult != null)
            {
                bool timeout = !oResult.AsyncWaitHandle.WaitOne(CALL_TIMEOUT);

                if (timeout)
                {
                    Log.WriteInfo("ParallelCall : EndInvoke : timeout");
                }
            }

            return oEndInvokeMethod.Invoke(
                pfDelegate,
                new object[] { oResult }
                );
        }

        private static MethodInfo GetMethod(Delegate pfDelegate, string sMethod)
        {
            Type oDelegateType = pfDelegate.GetType();

            MethodInfo oMethod =
                oDelegateType.GetMethod(sMethod);

            if (oMethod == null)
            {
                throw new TisException("Method [{0}] not found in delegate class [{1}]",
                    sMethod, pfDelegate);
            }

            return oMethod;
        }

        private static object[] BuildAsyncParams(object[] Params, AsyncCallback callBack, object userObject)
        {
            // Async invoke params require two more arguments
            ArrayBuilder oParams = new ArrayBuilder();
            oParams.AddRange(Params);
            oParams.Add(callBack);
            oParams.Add(userObject);
            return (object[])oParams.GetArray(typeof(object));
        }
    }

    #endregion
}
