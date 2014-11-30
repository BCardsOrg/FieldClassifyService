using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.FilePath
{
	[ComVisible(false)]
	public class SimplePathProvider: IPathProvider
	{
		private string m_Path;

		public SimplePathProvider(string sPath)
		{
			m_Path = sPath;
		}

		public string GetPath(string fileName)
		{
			return m_Path;
		}
	}
}
