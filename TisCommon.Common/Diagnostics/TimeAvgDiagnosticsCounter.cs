using System;

namespace TiS.Core.TisCommon.Diagnostics
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public class TimeAvgDiagnosticsCounter: BaseDiagnosticsCounter
	{
		private DateTime m_oLoadTime;

		private double   m_dTotal;
		
		public TimeAvgDiagnosticsCounter()
		{
			Clear();
		}
		
		public override double Value
		{
			get
			{
				TimeSpan oUpTime = DateTime.Now - m_oLoadTime;

				double dPPM = m_dTotal / oUpTime.TotalMinutes;

				return dPPM;
			}
		}

		public override void AddSample(double dValue)
		{
			m_dTotal += dValue;

            base.AddSample(dValue);
		}

		public override void Clear()
		{
			m_oLoadTime = DateTime.Now;
			m_dTotal    = 0;
		}

	}
}
