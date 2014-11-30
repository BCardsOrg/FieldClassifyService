using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.FilePath
{
    [ComVisible(false)]
    public interface IPathProvider
    {
        string GetPath(string sFileName);
    }
}
