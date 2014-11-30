using System;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon
{
	public class TrackedCodeBlock: IDisposable
	{
		private DateTime m_oStart;
		
		private string m_sMsg;

		private Log.Severity m_enLogSeverity;

        private bool m_writeLeave = true;

		public TrackedCodeBlock(
			string sMsgFormat,
			params object[] Params) : this(Log.Severity.DEBUG, sMsgFormat, Params)
		{
		}

		public TrackedCodeBlock(
			System.Reflection.MethodBase oMethod,
			string sMsgFormat,
			params object[] Params)
			:this(ReflectionUtil.GetFullMethodName(oMethod) + ": " + sMsgFormat, Params)
		{
		}

		public TrackedCodeBlock(
			Log.Severity enLogSeverity,
			string sMsgFormat,
            params object[] Params)
            : this(enLogSeverity, true, sMsgFormat, Params)
		{
			m_oStart = DateTime.Now;

			m_sMsg = String.Format(sMsgFormat, Params);

			m_enLogSeverity = enLogSeverity;

			WriteEnter();
		}

        public TrackedCodeBlock(
            Log.Severity enLogSeverity,
            bool writeEnter,
            string sMsgFormat,
            params object[] Params)
        {
            m_oStart = DateTime.Now;

            m_sMsg = String.Format(sMsgFormat, Params);

            m_enLogSeverity = enLogSeverity;

            if (writeEnter)
            {
                WriteEnter();
            }
        }

        #region IDisposable Members

		public void Dispose()
		{
            if (m_writeLeave)
            {
                WriteLeave();
            }
		}

		#endregion

        public bool DoWriteLeave
        {
            get { return m_writeLeave; }
            set { m_writeLeave = value; }
        }

        //
		//	Private
		//

		private void WriteEnter()
		{
			Log.Write(m_enLogSeverity, m_sMsg + ", Enter");
		}

		private void WriteLeave()
		{
			long lTicks = DateTime.Now.Ticks - m_oStart.Ticks;

			TimeSpan oSpan = TimeSpan.FromTicks(lTicks);

			Log.Write(
				m_enLogSeverity, 
				String.Format("{0}: Leave, took {1}", m_sMsg, oSpan));
		}

	}
}
