using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
	internal class FoLearn
	{
		[DllImport("FoLrn32.dll")]
		public static extern int CopyToNewEfi(
			[MarshalAs(UnmanagedType.LPStr)] string sName, 
			[MarshalAs(UnmanagedType.LPStr)] string sNewName
			);
	}
}
