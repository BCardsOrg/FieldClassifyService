using System;

namespace TiS.Core.TisCommon.Diagnostics
{
	[System.Runtime.InteropServices.ComVisible(false)]
    public delegate void DiagnosticsCounterSampleAddedDelegate(object sender);

	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IDiagnosticsCounter
	{
		//string Name { get; }
		double Value { get; }

		void AddSample(double dValue);

		void Clear();

        event DiagnosticsCounterSampleAddedDelegate OnSampleAdded;
	}
}
