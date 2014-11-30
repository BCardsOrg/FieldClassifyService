using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon
{
	[ComVisible(false)]
	public struct AutoHashCode
	{
		[NonSerialized]
		private int m_nHashCode;
		
		// Try to save space (static) & optimize performance
		[ThreadStatic]
		private static Random m_oRndGen;

		public override int GetHashCode()
		{
			if(m_nHashCode == 0)
			{
				if(m_oRndGen == null)
				{
					m_oRndGen = new Random();				
				}

				// Start from 1, "0" treated as invalid hash code
				m_nHashCode = m_oRndGen.Next(1, int.MaxValue);
			}

			return m_nHashCode;
		}

	}

	[ComVisible(false)]
	[Serializable]
	public class AutoHashedObject
	{
		[NonSerialized]
		private AutoHashCode m_oAutoHashCode = new AutoHashCode();

		public override int GetHashCode()
		{
			return m_oAutoHashCode.GetHashCode ();
		}

	}
}
