using System;

namespace TiS.Core.TisCommon.Diagnostics
{
	public class AvgDiagnosticsCounter: BaseDiagnosticsCounter
	{
		private long   m_lCount;
		private double m_dTotalValue; 

		public AvgDiagnosticsCounter()
		{
			Clear();
		}

		public override double Value
		{
			get
			{
				return m_dTotalValue /((double)m_lCount);
			}
		}

		public override void AddSample(double dValue)
		{
			m_lCount++;
			m_dTotalValue+=dValue;

            base.AddSample(dValue);
        }

		public override void Clear()
		{
			m_dTotalValue = 0;
			m_lCount      = 0;
		}

	}
}
