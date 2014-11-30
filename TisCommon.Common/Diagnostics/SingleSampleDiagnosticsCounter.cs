using System;

namespace TiS.Core.TisCommon.Diagnostics
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public class SingleSampleDiagnosticsCounter: BaseDiagnosticsCounter
	{
		private double   m_dValue;

		public SingleSampleDiagnosticsCounter()
		{
			Clear();
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
			m_dValue = dValue;

            base.AddSample(dValue);
        }

		public override void Clear()
		{
			m_dValue = 0;
		}

	}
}
