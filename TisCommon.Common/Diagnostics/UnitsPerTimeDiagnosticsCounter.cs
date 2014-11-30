using System;

namespace TiS.Core.TisCommon.Diagnostics
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public class UnitsPerTimeDiagnosticsCounter: BaseDiagnosticsCounter
	{
		double   m_dHistoryWeight = 0.5;
		double   m_dValue;
		TimeSpan m_oTimeInterval = new TimeSpan(0, 1, 0); // 1 min

		DateTime m_oLastSampleTime;

		private const double MAX_HISTORY_WEIGHT = 0.99;

		public UnitsPerTimeDiagnosticsCounter(
			double   dHistoryWeight,
			TimeSpan oTimeInterval
			)
		{
			if(dHistoryWeight > MAX_HISTORY_WEIGHT)
			{
				throw new TisException(
					"Invalid history weight [{0}], must be < [{1}]",
					dHistoryWeight,
					MAX_HISTORY_WEIGHT
					);
			}

			// m_oHistoryTime   = oHistoryTime;
			m_dHistoryWeight = dHistoryWeight;

			m_oTimeInterval = oTimeInterval;
		}

		public UnitsPerTimeDiagnosticsCounter()
		{

		}

		public override double Value
		{
			get
			{
				return m_dValue;
			}
		}

		public override void AddSample(double dValue)
		{
			DateTime oNow = DateTime.Now;

			TimeSpan oTimeFromLastSample = oNow - m_oLastSampleTime;


			if(m_dValue == 0 || oTimeFromLastSample.TotalMilliseconds == 0)
			{
				m_dValue = dValue;
			}
			else
			{
				double dUnitsPerMs = 
					dValue / oTimeFromLastSample.TotalMilliseconds;
				
				double dUnitsPerTimeInterval = 
					dUnitsPerMs * m_oTimeInterval.TotalMilliseconds;

				m_dValue = m_dValue * m_dHistoryWeight + 
					(1-m_dHistoryWeight) * dUnitsPerTimeInterval;
			}

			m_oLastSampleTime = oNow;

            base.AddSample(dValue);
        }

		public override void Clear()
		{
			m_oLastSampleTime = DateTime.Now;
			m_dValue		  = 0;
		}

	}
}
