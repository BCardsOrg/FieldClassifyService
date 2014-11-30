using System;
using System.Collections;
	
namespace TiS.Core.TisCommon
{
	public class EmptyArrays
	{
		// Type -> Array map
		private static Hashtable m_oEmptyArrays = new Hashtable();

		public static Array GetForElementType(Type oElementType)
		{
			Array oArray = null;

			lock(typeof(EmptyArrays))
			{
				oArray = (Array)m_oEmptyArrays[oElementType];

				if(oArray == null)
				{
					oArray = Array.CreateInstance(oElementType, 0);

					m_oEmptyArrays[oElementType] = oArray;
				}
			}

			return oArray;
		}

		public static byte[] ByteArray
		{
			get
			{
				return (byte[])GetForElementType(typeof(byte));
			}
		}

		public static short[] ShortArray
		{
			get
			{
				return (short[])GetForElementType(typeof(short));
			}
		}

		public static int[] IntArray
		{
			get
			{
				return (int[])GetForElementType(typeof(int));
			}
		}

		public static char[] CharArray
		{
			get
			{
				return (char[])GetForElementType(typeof(char));
			}
		}

		public static string[] StringArray
		{
			get
			{
				return (string[])GetForElementType(typeof(string));
			}
		}

		public static object[] ObjectArray
		{
			get
			{
				return (object[])GetForElementType(typeof(object));
			}
		}

	}
}
