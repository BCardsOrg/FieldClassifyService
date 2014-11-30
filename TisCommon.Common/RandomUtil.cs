using System;

namespace TiS.Core.TisCommon
{
   
	public class RandomUtil
	{
		public static ulong GetUInt64()
		{
			Random oRnd = new Random();

			byte[] RndBytes = new byte[8];

			oRnd.NextBytes(RndBytes);

			return BitConverter.ToUInt64(RndBytes, 0);
		}
	}
}
