using System;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using TiS.Core.TisCommon.Configuration;
using System.Data;
using System.ServiceModel;
using System.Threading;

namespace TiS.Core.TisCommon.Storage
{
    [Serializable]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class BaseStorageImpl : IStorageService
    {
        // Base data path 
        private string m_basePath = "";
        private string m_relativePath = "";

        private const int READ_RETRIES = 3;
         
        
        public BaseStorageImpl(string basePath)
        {
            m_basePath = basePath;
            RetryWaitSpan = TimeSpan.FromSeconds(1);
        }

        public TimeSpan RetryWaitSpan { set; get; }
        
        #region Set base path


        protected virtual string CreateRelativePath()
        {
            return "";
        }


         public void SetRelativePath(string relativePath)
        {
            m_relativePath = relativePath;
        }

		 protected string CalculateLocalPath(string fileName)
		 {
			 string basePath = SetBasePath();

			 string localFile = Path.Combine(basePath, fileName);

			 return localFile;
		 }

		 protected string GetLocalPath(string fileName)
        {
			string localFile = CalculateLocalPath(fileName);

			EnsurePathExists(Path.GetDirectoryName(localFile));

            return localFile;
        }

        protected string GetLocalDirectory(string path)
        {
            string basePath = SetBasePath();

            string localDir = Path.Combine(basePath, StringUtil.NullToEmpty(path));

            return localDir;
        }

        private string SetBasePath()
        {
            if (String.IsNullOrEmpty(m_basePath))
            {
				throw new TisException("Base path is empty in {0}", this.GetType().ToString());
            }

            if (String.IsNullOrEmpty(m_relativePath))
            {
                m_relativePath = CreateRelativePath();
            }

            string fullBasePath = Path.Combine(m_basePath, m_relativePath);

            return fullBasePath;
        }       
        #endregion

        #region IStorageService methods

        /// <summary>
        /// Checks whether specific file exists 
        /// </summary>
        /// <param name="path">Path to the storage</param>
        /// <param name="fileName">File name</param>
        /// <returns>True if file found.</returns>
        public virtual bool IsStorageExist(string fileName)
        {
			string localFile = CalculateLocalPath(fileName);

            bool fileExists = File.Exists(localFile);

            return fileExists;
        }

        /// <summary>
        /// Creates directory
        /// </summary>
        /// <param name="path">New directory path</param>
        public virtual void CreateDir(string path)
        {
            string localPath = GetLocalPath(path);

            EnsurePathExists(localPath);
        }

        public virtual void DeleteDir(string path)
        {
            string localPath = GetLocalDirectory(path);

            if (Directory.Exists(localPath))
            {
                Directory.Delete(localPath, true);
            }
        }

        public virtual void CleanUp(string path)
        {
            string localPath = GetLocalPath(path);

            DeleteEmptyDirs(localPath);
        }

        public virtual string[] QuerySubDirs(string path, bool bRecursive)
        {
            return QuerySubDirs(path, bRecursive, DirectoryView.Flat);
        }
        public virtual string[] QuerySubDirs(string path, bool bRecursive, DirectoryView enDirectoryView)
        {
            string[] Dirs = null;

            string basePath = SetBasePath();

            if (!bRecursive)
            {
                // Get local path
                string localPath = Path.Combine(basePath, path);

                // If path exists
                if ( Directory.Exists(localPath))
                {
                    // One level query - simple
                    string[] FullPathDirs = Directory.GetDirectories(localPath, "*.*");

                    Dirs = new string[FullPathDirs.Length];

                    for (int i = 0; i < FullPathDirs.Length; i++)
                    {
                        string sDirName = FullPathDirs[i];

                        Dirs[i] = GetLowerLevelDir(sDirName);
                    }
                }
                else
                {
                    // Return empty array
                    Dirs = EmptyArrays.StringArray;
                }
            }
            else
            {
                // Recursive query

                // Get one direct subdirs of the provided path
                string[] OneLevelSubDirs = QuerySubDirs(path, false);

                List<string> AllSubDirs = new List<string>();

                // Perform recursive query on the direct subdirs
                foreach (string sSubDir in OneLevelSubDirs)
                {
                    string sSubDirPath = String.Empty;

                    if (path != String.Empty)
                    {
                        sSubDirPath = String.Format(
                            "{0}{1}{2}",
                            path,
                            Path.DirectorySeparatorChar,
                            sSubDir);
                    }
                    else
                    {
                        sSubDirPath = sSubDir;
                    }

                    if (basePath != String.Empty && sSubDirPath.IndexOf(basePath) > -1)
                    {
                        sSubDirPath =
                            sSubDirPath.Substring(basePath.Length + 1,
                                                  sSubDirPath.Length - basePath.Length - 1);
                    }

                    if (enDirectoryView == DirectoryView.Flat)
                    {
                        AllSubDirs.Add(sSubDir);
                    }
                    else
                    {
                        AllSubDirs.Add(sSubDirPath);
                    }

                    AllSubDirs.AddRange(
                        QuerySubDirs(sSubDirPath, true, enDirectoryView));
                }

                // Get all as array
                Dirs = AllSubDirs.ToArray();
            }

            return Dirs;
        }


        public virtual string[] QueryStorage(string path, string filter)
        {
			string localPath = CalculateLocalPath(path);

            // If directory not exists
            if (!Directory.Exists(localPath))
            {
                // Return empty array
                return EmptyArrays.StringArray;
            }

            // Get list of files 
            string[] FullFileNames = Directory.GetFiles(
                localPath,
                filter);

            List<string> BLOBNames = new List<string>();

            // Fill file names array
            foreach (string sFullFileName in FullFileNames)
            {
                BLOBNames.Add(Path.GetFileName(sFullFileName));
            }

            return BLOBNames.ToArray();
        }


        public virtual StorageInfo GetStorageInfo(string filePath, StorageInfoFlags enDataFlags)
        {
			string localFileName = CalculateLocalPath(filePath);

            FileInfo fileInfo = new FileInfo( localFileName);

            long lSizeBytes = 0;
            int nCRC32 = 0;
            DateTime oLastModified = DateTime.Now;

            if (fileInfo.Exists)
            {
                if (StorageInfo.FlagSet(enDataFlags, StorageInfoFlags.Size))
                {
                    lSizeBytes = fileInfo.Length;
                }

                if (StorageInfo.FlagSet(enDataFlags, StorageInfoFlags.LastModified))
                {
                    oLastModified = fileInfo.LastWriteTime;
                }
            }

            string fileName = Path.GetFileName(filePath);
            StorageInfo storageInfo = new StorageInfo(
                fileName,
                lSizeBytes,
                nCRC32,
                oLastModified,
                enDataFlags);

            return storageInfo;
        }

  
        public virtual void RenameStorage(
            string fileName,
            string newFileName)
        {

            File.Move(
				CalculateLocalPath(fileName),
                GetLocalPath(newFileName));
        }      

        public virtual void DeleteFile( string fileName)
        {
			string localPath = CalculateLocalPath(fileName);

            File.Delete(localPath);
        }


        public virtual StorageInfo[] QueryStorageInfo(string path, string sFilter, StorageInfoFlags dataFlags, bool withSubDirs = false)
        {
            string[] BLOBs = QueryStorage(path, sFilter);

            List<StorageInfo> storagesInfo = new List<StorageInfo>();

            for (int i = 0; i < BLOBs.Length; i++)
            {
                storagesInfo.Add(GetStorageInfo(Path.Combine(path, BLOBs[i]), dataFlags));
            }

            if (withSubDirs)
            {
                string[] subDirs = QuerySubDirs(String.Empty, true);

                foreach (var subdir in subDirs)
                {
                    StorageInfo[] subDirsStoragesInfo =
                        QueryStorageInfo(subdir, sFilter, StorageInfoFlags.Size | StorageInfoFlags.LastModified);

                    foreach (StorageInfo storageInfo in subDirsStoragesInfo)
                    {
                        storagesInfo.Add(new StorageInfo(
                            Path.Combine(subdir, storageInfo.Name),
                            storageInfo.SizeBytes,
                            storageInfo.CRC32,
                            storageInfo.LastModified,
                            storageInfo.DataFlags));
                    }
                }
            }

            return storagesInfo.ToArray();
        }
       
 
        public virtual void DeleteStorage(string[] fileNames)
        {
            if (fileNames == null) throw new ArgumentNullException("blobNames");

            string basePath = SetBasePath();

            // Delete all specified BLOBs
            foreach (string file in fileNames)
            {
                string localPath = Path.Combine(basePath, file);

                File.Delete(localPath);
            }
        }

        #region Read / write from storage

        public virtual byte[] ReadStorage(string fileName)
        {
            string localPath = CalculateLocalPath(fileName);

            var result = FileUtil.ReadAllBytes(localPath);
            
            return result;
        }

        public virtual byte[] ReadPartialStorage(string fileName, long offset, int length)
        {
            string localPath = CalculateLocalPath(fileName);

            using (FileStream stream = File.OpenRead(localPath)) 
            {
                byte[] data = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                int bytes = stream.Read(data, 0, length);

                if (bytes != length)
                {
                    Array.Resize<byte>(ref data, bytes);
                }

                return data;
            }
        }

        public virtual void WriteStorage(string fileName, byte[] data)
        {
            string localPath = GetLocalPath(fileName);

            FileUtil.WriteAllBytes(localPath, data);
        }

        public virtual void WritePartialStorage(string fileName, long offset, byte[] data)
        {
            string localPath = GetLocalPath(fileName);

            using (FileStream stream = File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Write(data, 0, data.Length);
            }
        }

        public virtual void WriteChunkedStorage(string fileName, Queue<MemoryDataChunk> memoryDataChunks)
        {
            string localPath = GetLocalPath(fileName);

            using (FileStream fileStream = new FileStream(localPath, FileMode.Create))
            {
                WriteChunkedStorage(fileStream, memoryDataChunks);
            }
        }
  
        public virtual Stream ReadStream(string fileName)
        {
			string localPath = CalculateLocalPath(fileName);

            return File.OpenRead(localPath);
        }

        /// <summary>
        /// Writes dataset object as XML
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <remarks>Not a part of the service contract</remarks>
		public virtual void WriteDataSet(DataSet data, string fileName, bool includeSchema = false)
        {
            string localFile = GetLocalPath(fileName);

            // Open stream
            using (Stream oStream = new FileStream(localFile, FileMode.Create, FileAccess.Write))
            {
                // Create reader
                using (StreamWriter oWriter = new StreamWriter(oStream))
                {
                    // store data
                    data.WriteXml(oWriter, includeSchema ? XmlWriteMode.WriteSchema : XmlWriteMode.IgnoreSchema);
                }
            }
        }

		public virtual System.Data.DataSet ReadDataSet(string fileName)
		{
			System.Data.DataSet data = new System.Data.DataSet();

			string localFile = GetLocalPath(fileName);

			using (StreamReader oReader = new StreamReader(localFile))
			{
				// store data
				data.ReadXml(oReader);
			}

			return data;
		}
		#endregion

        #endregion

        protected virtual void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        protected void WriteChunkedStorage(
            FileStream fileStream, 
            Queue<MemoryDataChunk> memoryDataChunks)
        {
            while (memoryDataChunks.Count > 0)
            {
                var memoryDataChunk = memoryDataChunks.Dequeue();

                fileStream.Write(memoryDataChunk.Data, 0, memoryDataChunk.Size);

                memoryDataChunk.Data = null;
                memoryDataChunk = null;
            }
        }

        #region Private methods

        private void DeleteEmptyDirs(string rootPath)
        {
            string[] DirNames = Directory.GetDirectories(rootPath);

            foreach (string dirName in DirNames)
            {
                if (Directory.GetFiles(dirName).Length == 0 && Directory.GetDirectories(dirName).Length == 0)
                {
                    Directory.Delete(dirName);
                }
                else
                {
                    DeleteEmptyDirs(dirName);
                }
            }

            if (Directory.GetFiles(rootPath).Length == 0 && Directory.GetDirectories(rootPath).Length == 0)
            {
                Directory.Delete(rootPath);
            }
        }

        private string GetLowerLevelDir(string path)
        {
            int nSepIndex = path.LastIndexOf(
                Path.DirectorySeparatorChar);

            if (nSepIndex >= 0)
            {
                return path.Substring(nSepIndex + 1);
            }

            return path;
        }

        #endregion
    }
  
}
