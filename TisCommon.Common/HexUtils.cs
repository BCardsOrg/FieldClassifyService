using System;

namespace TiS.Core.TisCommon
{
	public class HexUtils
	{
		static char[] hexDigits = {
									  '0', '1', '2', '3', '4', '5', '6', '7',
									  '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};


		public static byte[] FromHexString(string sHexString)
		{
			if(sHexString.Length % 2 > 0)
			{
				throw new ArgumentException("Invalid string", "sHexString");
			}

			int nBytes = sHexString.Length / 2;

			byte[] RetBytes = new byte[nBytes];

			for(int i=0; i<nBytes; i++)
			{
				byte bHigh = HexCharToByte(sHexString[i*2]);	
				byte bLow  = HexCharToByte(sHexString[i*2+1]);	

				RetBytes[i] = (byte)(bHigh * (byte)16 + bLow);
			}

			return RetBytes;
		}

		public static string ToHexString(byte[] bytes) 
		{
			try
			{
				char[] chars = new char[bytes.Length * 2];
				for (int i = 0; i < bytes.Length; i++) 
				{
					int b = bytes[i];
					chars[i * 2] = hexDigits[b >> 4];
					chars[i * 2 + 1] = hexDigits[b & 0xF];
				}
				return new string(chars);
			}
			catch(Exception oExc)
			{
				System.Diagnostics.Trace.WriteLine(oExc.Message);
				return null;
			}			
		}

		//
		// Private
		//

		private static byte HexCharToByte(char cChar)
		{
			int nIndex = Array.IndexOf(hexDigits, cChar);

			if(nIndex < 0)
			{
				throw new ArgumentException("Invalid char", "cChar");
			}

			return (byte)nIndex;
		}
	}
}
