using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.Storage
{
	public static class StaticStorageServices
	{
        // Default chunk size - 16_MB
        public const int DEFAULT_CHUNK_SIZE = 16777216;

        public static void Copy(
			IStorageService oSource,
			IStorageService oDestination,
			string		 sFilter,
			bool		 bIncludeSubDirs,
            List<string> selectedSubDirs = null,
            List<string> excludedSubDirs = null,
            List<string> excludedFiles = null,
            bool preserveDirStructure = true,
            List<string> SubDirsToPreserve = null)
		{
			string[] DirsToCopy = GetDirectoriesIncludingRoot(
				oSource,
				"",	// Root
				bIncludeSubDirs,
                selectedSubDirs);
			
			foreach(string sDir in DirsToCopy)
			{
                if (excludedSubDirs != null && excludedSubDirs.Contains(sDir))
                {
                    continue;
                }

				// Copy directories
				CopyDir(
					oSource, 
					oDestination, 
					sDir,	
					sFilter,
                    excludedFiles,
                    preserveDirStructure,
                    SubDirsToPreserve);
			}
		}

		public static void Copy(int chunkSize, IStorageService source, IStorageService destination, string sourceBLOBName, string destBLOBName)
		{
            // Open target & destination streams
            byte[] buffer = null;
            int offsetIndex = 0;

            do
            {
                int offset = chunkSize * offsetIndex++;

                buffer = source.ReadPartialStorage(sourceBLOBName, offset, chunkSize);
                destination.WritePartialStorage(destBLOBName, offset, buffer);
            }
            while (buffer.Length == chunkSize);
		}

        public static void Copy(IStorageService source, IStorageService destination, string sourceBLOBName, string destBLOBName)
        {
            var storageInfo = source.GetStorageInfo(sourceBLOBName, StorageInfoFlags.All);

            if (storageInfo.SizeBytes > DEFAULT_CHUNK_SIZE)
            {
                Copy(DEFAULT_CHUNK_SIZE, source, destination, sourceBLOBName, destBLOBName);
            }
            else
            {
                var buffer = source.ReadStorage(sourceBLOBName);
                destination.WriteStorage(destBLOBName, buffer);
            }
        }

		public static void DeleteStorage(
			IStorageService storage,
			string		 path,
			string		 sFilter,
            bool bIncludeSubDirs,
            List<string> selectedSubDirs = null)
		{
			string[] DirsToDelete = GetDirectoriesIncludingRoot(
				storage,
				path,	// Root
				bIncludeSubDirs,
                selectedSubDirs);

			foreach(string sDir in DirsToDelete)
			{
				// Copy directories
				DeleteDirFiles(
					storage, 
					sDir,	
					sFilter);
			}
		}

		public static string[] GetUsedExtenstions(
			IStorageService storage,
			string		 path,
			bool		 bIncludeSubDirs)
		{
			Hashtable oUsedExtensions = new Hashtable();
			
			// Get all required directories
			string[] Dirs = GetDirectoriesIncludingRoot(
				storage,
				path,	// Root
				bIncludeSubDirs);

			foreach(string sDir in Dirs)
			{
				// Query for BLOB names in directory
				string[] BLOBs = storage.QueryStorage(
					sDir,
					"*.*"
					);

				foreach(string sBLOB in BLOBs)
				{
					// Get BLOB extension
					string sExt = Path.GetExtension(sBLOB);

					// If valid
					if(sExt.Length > 0)
					{
						// Keep extension in dictionary
						oUsedExtensions[sExt] = true;	
					}
				}
			}

			return (string[])ArrayBuilder.CreateArray(oUsedExtensions.Keys, typeof(string), null);
		}

        public static byte[] ReadStorage(
                IStorageService storage,
                string storageName)
        {
            byte[] data = storage.ReadStorage(storageName);
            return data;
        }

        #region Private methods

        private static string[] GetDirectoriesIncludingRoot(
            IStorageService oStorage,
            string sPath,
            bool bIncludeSubDirs,
            List<string> selectedSubDirs = null)
        {
            List<string> Dirs = new List<string>();

            if (!bIncludeSubDirs || selectedSubDirs == null || selectedSubDirs.Count == 0)
            {
                // Root directory is always copied
                Dirs.Add(sPath);
            }

            if (bIncludeSubDirs)
            {
                // Query for subdirs
                Dirs.AddRange(oStorage.QuerySubDirs(sPath, true, DirectoryView.Tree));

                if (selectedSubDirs != null && selectedSubDirs.Count > 0)
                {
                    string[] foundFirs = SetUtil<string>.GetIntersection(Dirs, selectedSubDirs);

                    Dirs.Clear();

                    Dirs.AddRange(foundFirs);
                }
            }

            return Dirs.ToArray();
        }

        private static void CopyDir(
            IStorageService oSource,
            IStorageService oDestination,
            string sPath,
            string sFilter,
            List<string> excludedFiles = null,
            bool preserveDirStructure = false,
            List<string> SubDirsToPreserve = null)
        {
            string[] BLOBs = oSource.QueryStorage(sPath, sFilter);

            foreach (string sBLOB in BLOBs)
            {
                if (excludedFiles == null || !excludedFiles.Contains(sBLOB))
                {
                    var targetPath = preserveDirStructure || (SubDirsToPreserve != null && SubDirsToPreserve.Contains(sPath)) ? Path.Combine(sPath, sBLOB) : sBLOB;

                    Copy(
                        oSource,
                        oDestination,
                        Path.Combine(sPath, sBLOB),
                        targetPath);
                }
            }
        }

        private static void DeleteDirFiles(
            IStorageService storage,
            string path,
            string filter)
        {
            // Query for files that match the filter
            string[] fileNames = storage.QueryStorage(path, filter);

            // Delete files
            storage.DeleteStorage(fileNames);
        } 
        #endregion
	}

}
