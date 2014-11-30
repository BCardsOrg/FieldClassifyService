using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.FilePath
{
    [Guid("7D781C6F-F8B0-4E6F-A369-1152405130D9")]
    public interface ITisClientPathLocator
    {
        string this[String sExtension] { get; }
		string Path(string sExtension);
        string GetPath(string sExtension);
        void SavePathMap();
    }
}



