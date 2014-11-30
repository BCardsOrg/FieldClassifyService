using System;

namespace TiS.Core.TisCommon.Diagnostics
{
	[System.Runtime.InteropServices.ComVisible(false)]
	public abstract class BaseDiagnosticsCounter: IDiagnosticsCounter 
	{
		public BaseDiagnosticsCounter()
		{
		}

		public abstract double Value { get; }

        public virtual void AddSample(double dValue)
        {
            if (OnSampleAdded != null)
            {
                OnSampleAdded(this);
            }
        }

		public abstract void Clear();

        public event DiagnosticsCounterSampleAddedDelegate OnSampleAdded;
    }
}
