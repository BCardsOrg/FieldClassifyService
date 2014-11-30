using System;

namespace TiS.Core.TisCommon
{
	public class BitUtil
	{
		public static bool IsBitSet(uint nVal, uint nMask)
		{
			return ((nVal & nMask) > 0);
		}
	}
}
