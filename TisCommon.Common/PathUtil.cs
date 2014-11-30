using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace TiS.Core.TisCommon
{
	public class PathUtil
	{
		public static string GetExePath()
		{
			// Return main module directory
			return Path.GetDirectoryName(GetFullExeName());
		}

		public static string GetExeName()
		{
			return Path.GetFileName(GetFullExeName());
		}

		public static string GetFullExeName()
		{
			return ProcessUtil.CurrentProcessMainModule;
		}

		public static void EnsurePathExist(string sPath)
		{
			if(!Directory.Exists(sPath))
			{
				Directory.CreateDirectory(sPath);
			}
		}

		public static string GetWinDir()
		{
			string sSys32Dir = Environment.GetFolderPath(
				Environment.SpecialFolder.System);

			string sWinDir = Path.GetFullPath(
				Path.Combine(sSys32Dir, "..\\"));

			return sWinDir;
		}

        public static string EnsurePathStartsWith(string path, string prefix)
        {
            return ((path.StartsWith(prefix)) ? String.Empty : prefix).ToString() + path;
        }

		public static bool IsValidPathName(string pathName, bool fullPath)
		{
			if (fullPath == false)
			{
				return
					string.IsNullOrEmpty(pathName) == false &&
					string.Compare(pathName, "con", true) != 0 &&
					string.Compare(pathName.Substring(0, 4), "con.", true) != 0 &&
					Path.GetInvalidFileNameChars().Any(x =>
					{
						return pathName.Contains(x) == true;
					}) == false &&
					Path.GetInvalidPathChars().Any(x =>
					{
						return pathName.Contains(x) == true;
					}) == false;
			}
			else
			{
				bool result = false;
				try
				{
					result =
						string.IsNullOrEmpty(pathName) == false &&
						Path.GetInvalidPathChars().Any(x =>
						{
							return pathName.Contains(x) == true;
						}) == false;
					if (result == true)
					{
						if ( Path.IsPathRooted(pathName) == true)
						{
							result = true;
						}
					}
				}
				catch {}
				
				return result;
			}
		}
    }
}
