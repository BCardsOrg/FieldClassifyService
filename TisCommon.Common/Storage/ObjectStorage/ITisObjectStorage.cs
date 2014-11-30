using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Storage.ObjectStorage
{
	[Guid("25D03D00-0F56-4769-8041-20F3CCA82059")]
  // [CLSCompliant(false)]
	public interface ITisObjectStorage : IDisposable
	{
		object   LoadObject(string sName);
		void	 StoreObject(object oObj, string sName);
		void	 DeleteObject(string sName);
		bool	 IsObjectExists(string sName);
		string[] QueryObjects(string sNameFilter);
	}
}
