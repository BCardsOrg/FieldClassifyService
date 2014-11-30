
using System;
using System.IO;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
	public class StreamUtil
	{
		private const int DEFAULT_BUFF_SIZE = 1024*512;

		public static void CopyData(Stream oFrom, Stream oTo)
		{
			CopyData(oFrom, oTo, DEFAULT_BUFF_SIZE);
		}

		public static void CopyData(Stream oFrom, Stream oTo, int nBlockSize)
		{
			byte[] Buff = new byte[nBlockSize];
			
			int nLen = 0;

			while( (nLen = oFrom.Read(Buff, 0, Buff.Length)) > 0)
			{
				oTo.Write(Buff, 0, nLen);
			}

		}

		public static long GetBytesLeft(Stream oStream)
		{
			return oStream.Length - oStream.Position;
		}

		public static byte[] ReadToEnd(Stream oStream)
		{
			byte[] Data = new byte[GetBytesLeft(oStream)];

			int nBytesRead = oStream.Read(Data, 0, Data.Length);

			if(Data.Length == nBytesRead)
			{
				return Data;
			}

			byte[] TruncData = new byte[nBytesRead];
		
			Array.Copy(Data, TruncData, nBytesRead);		

			return TruncData;
		}

	}
}
