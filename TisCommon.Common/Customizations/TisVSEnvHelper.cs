using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;

namespace TiS.Core.TisCommon.Customizations
{
    #region TisVSEnvHelper

    public class TisVSEnvHelper
	{
		public static string GetFullFileName (string sFileName, string sFileExt)
		{
			if (Path.HasExtension(sFileName))
			{
                if (StringUtil.CompareIgnoreCase(Path.GetExtension(sFileName), sFileExt))
				{
					return sFileName;
				}
				else
				{
					return Path.ChangeExtension(sFileName, sFileExt);
				}
			}
			else
			{
				return sFileName + sFileExt;
			}
		}

		public static string GetFullFileName (string sFileName, string sFileDir, string sFileExt)
		{
			if (!Path.IsPathRooted(sFileName))
			{
				sFileName = Path.Combine (sFileDir, sFileName);
			}

			if (Path.HasExtension(sFileName))
			{
				if (StringUtil.CompareIgnoreCase(Path.GetExtension(sFileName), sFileExt))
				{
					return sFileName;
				}
				else
				{
					return Path.ChangeExtension(sFileName, sFileExt);
				}
			}
			else
			{
				return sFileName + sFileExt;
			}
		}

		public static void SaveFile(string sFileName, string sContent, bool bOverwriteIfExists)
		{
			StreamWriter oFileWriter = null;

			if (File.Exists (sFileName) && !bOverwriteIfExists)
			{
				oFileWriter = File.AppendText (sFileName);

				oFileWriter.WriteLine();
			}
			else
			{
				oFileWriter = File.CreateText (sFileName);
			}

			using (oFileWriter)
			{
				oFileWriter.Write(sContent);
			}
		}

		public static void DeleteFiles (string sRootDir, string sSearchPattern)
		{
			string[] Files = FindFiles(sRootDir, sSearchPattern);

			foreach (string sFile in Files)
			{
				File.Delete (sFile);
			}
		}

		public static void SafeFileMove(string sSrcFile, string sDstFile)
		{
			if (StringUtil.IsStringInitialized(sSrcFile) && File.Exists(sSrcFile) &&
				StringUtil.IsStringInitialized(sDstFile))
			{
				if (File.Exists(sDstFile))
				{
					File.Delete(sDstFile);
				}

				File.Move(sSrcFile, sDstFile);
			}
		}

		public static void SafeFilesMove(string sSrcsSearchPattern, string sFileExt)
		{
			if (StringUtil.IsStringInitialized(sSrcsSearchPattern) &&
				StringUtil.IsStringInitialized(sFileExt))
			{
				string[] Files = FindFiles(
					Path.GetDirectoryName(sSrcsSearchPattern), 
					Path.GetFileName(sSrcsSearchPattern));

				string sDstFile = String.Empty;

				foreach (string sSrcFile in Files)
				{
					sDstFile = Path.ChangeExtension(sSrcFile, sFileExt);

					SafeFileMove(sSrcFile, sDstFile);
				}
			}
		}

		public static string[] FindFiles(string sRootDir, string sSearchPattern)
		{
			DirectoryInfo RootDirInfo = new DirectoryInfo (sRootDir);

			FileInfo[] fis = RootDirInfo.GetFiles (sSearchPattern);

			ArrayBuilder oArrayBuilder = new ArrayBuilder(typeof(string));

			foreach (FileInfo fi in fis)
			{
				oArrayBuilder.AddIfNotExists(fi.FullName);
			}

			return (string[])oArrayBuilder.GetArray();
		}

        public static string[] FindFiles(string sRootDir, string sSearchPattern, bool bRecursive)
        {
            List<string> Files = new List<string>();

            FindFiles(sRootDir, sSearchPattern, bRecursive, Files);

            return Files.ToArray();
        }

        public static string RemoveChars(string sSource, char cCharToRemove)
		{
			return String.Concat(sSource.Split(new char[] {cCharToRemove}));
		}

		public static string GetEnumName (object oObject)
		{
			Type oType = oObject.GetType();

			if (Enum.IsDefined(oType, oObject))
			{
				return Enum.GetName(oType, oObject);
			}
			else
			{
				return String.Empty;
			}
		}

		public static string EnsureClassFullName (string sNamespace, string sClassName)
		{
			if (StringUtil.IsStringInitialized(sClassName))
			{
				if (sClassName.Split(new char[] {Type.Delimiter}).Length < 2)
				{
					if (StringUtil.IsStringInitialized(sNamespace))
					{
						return sNamespace + Type.Delimiter + sClassName;
					}
				}

				return sClassName;
			}
			else
			{
				return String.Empty;
			}
		}

		public static void ParseClassName (string sClassFullName, out string sNamespace, out string sName)
		{
			sNamespace = String.Empty;
			sName      = String.Empty;

			if (StringUtil.IsStringInitialized(sClassFullName))
			{
				string[] StringParts = sClassFullName.Split(new char[] {Type.Delimiter});

				if (StringParts.Length < 2)
				{
					sName = StringParts[0];
				}
				else
				{
					sNamespace = StringParts[0];
					sName      = StringParts[1];
				}
			}
		}

		public static string GetClassName (string sClassFullName)
		{
			string sNamespace;
			string sName;

			ParseClassName(sClassFullName, out sNamespace, out sName);

			return sName;
		}

		public static string GetClassNamespace (string sClassFullName)
		{
			string sNamespace;
			string sName;

			ParseClassName(sClassFullName, out sNamespace, out sName);

			return sNamespace;
		}

        public static bool GetAvailableDTEVersion(string DTEPreferedVersion, out string DTEAvailableVersion)
        {
            string sDTERegisryKey = String.Empty;
            bool isAvailable = false;

            DTEAvailableVersion = String.Empty;

            isAvailable = GetDTEVersionAvailability(DTEPreferedVersion);

            if (isAvailable)
            {
                DTEAvailableVersion = DTEPreferedVersion;
            }
            else
            {
                string DTECurrentVersionKey = Path.Combine(Registry.ClassesRoot.Name, Path.Combine("VisualStudio.DTE", "CurVer"));
                string DTECurrentVersion = (string)Registry.GetValue(DTECurrentVersionKey, String.Empty, null);

                isAvailable = GetDTEVersionAvailability(DTECurrentVersion);

                if (isAvailable)
                {
                    DTEAvailableVersion = DTECurrentVersion;
                }
            }

            return isAvailable;
        }

        public static void FindFiles(string sRootDir, string sSearchPattern, bool bRecursive, List<string> Files)
        {
            Files.AddRange(FindFiles(sRootDir, sSearchPattern));

            if (bRecursive)
            {
                DirectoryInfo RootDirInfo = new DirectoryInfo(sRootDir);

                DirectoryInfo[] dirs = RootDirInfo.GetDirectories();

                foreach (DirectoryInfo dir in dirs)
                {
                    FindFiles(dir.FullName, sSearchPattern, bRecursive, Files);
                }
            }
        }

        private static bool GetDTEVersionAvailability(string DTEVersion)
        {
            object DTERegisryKeyValue = null;

            if (DTEVersion != null)
            {
                string DTERegisryKey = (string)Path.Combine(Registry.ClassesRoot.Name, DTEVersion);

                DTERegisryKeyValue = Registry.GetValue(DTERegisryKey, String.Empty, null);
            }

            return DTERegisryKeyValue != null;
        }
    }

    #endregion
}
