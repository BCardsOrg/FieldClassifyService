using System;
using System.Reflection;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon
{

	[ComVisible(false)]
	public class ExceptionUtil
	{
		public static void RaiseArgumentException(
			string	    sParam, 
			string		sMessage, 
			object	    oVal,
			MethodBase  oMethod)
		{
			Log.WriteError("{0}: ArgumentException, {1}, Param=[{2}] Value=[{3}]",
				oMethod,
				sMessage,
				sParam,
				oVal);

			throw new ArgumentException(sMessage, sParam);
		}

		public static void RaiseArgumentNullException(
			string	    sParam, 
			string		sMessage, 
			MethodBase  oMethod)
		{
			Log.WriteError("{0}: ArgumentNullException, {1}, Param=[{2}]",
				oMethod,
				sMessage,
				sParam);

			throw new ArgumentNullException(sMessage, sParam);
		}

		public static void RaiseArgumentOutOfRangeException(
			string	    sParam,
			object      oValue,
			string		sMessage, 
			MethodBase  oMethod)
		{
			Log.WriteError("{0}: ArgumentOutOfRangeException, {1}, Param=[{2}], Value=[{3}]",
				oMethod,
				sMessage,
				sParam,
				oValue);

			throw new ArgumentOutOfRangeException(sMessage, oValue, sParam);
		}

		public static void RaiseIndexOutOfRangeException(
			object      oValue,
			string		sMessage, 
			MethodBase  oMethod)
		{
			Log.WriteError("{0}: IndexOutOfRangeException, {1}, Value=[{2}]",
				oMethod,
				sMessage,
				oValue);

			throw new IndexOutOfRangeException(sMessage);
		}

		public static void RaiseInvalidOperationException(
			MethodBase  oMethod,
			string		sFormat,
			params object[] Args)
		{
			InvalidOperationException oExc = new InvalidOperationException(
				GetExceptionMessage(sFormat, Args));

			LogException(
				oMethod,
				oExc);
			
			throw oExc;
		}

		//
		//	Private
		//

		private static void LogException(
			MethodBase  oMethod,
			Exception   oExc)
		{
			Log.WriteError("{0}: {1}, {2}",
				oMethod,
				oExc.GetType().Name,
				oExc.Message);
		}

		private static string GetExceptionMessage(
			string		    sFormat,
			params object[] Args)
		{
			try
			{
				return String.Format(sFormat, Args);
			}
			catch(FormatException )
			{
				return sFormat;
			}
		}
	}
}
