using System;

namespace TiS.Core.TisCommon.Storage
{
	[Flags]
	public enum StorageInfoFlags 
	{ 
		Size = 1, 
		LastModified = 2, 
		CRC32 = 4 ,
		All = Size | LastModified | CRC32
	}

	public interface IStorageInfo
	{
		string   Name		  { get; }
		long     SizeBytes	  { get; }
		DateTime LastModified { get; }
		int      CRC32		  { get; }

		bool     SizeValid         { get; }
		bool     LastModifiedValid { get; }
		bool     CRC32Valid        { get; }
	}
}
