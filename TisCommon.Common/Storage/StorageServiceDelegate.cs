using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveDs;
using System.IO;

namespace TiS.Core.TisCommon.Storage
{
    public class StorageServiceDelegate : ITransactionalStorage
    {
        private IStorageService m_StorageService;
        private string m_RelativePath;

        public StorageServiceDelegate(IStorageService storageService, string relativePath)
        {
            m_StorageService = storageService;
            m_RelativePath = relativePath;
        }

        #region ITransactionalStorage Members

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IStorageService Members

        public void DeleteDir(string path)
        {
            m_StorageService.DeleteDir(m_RelativePath);
        }

        public void CreateDir(string path)
        {
            m_StorageService.CreateDir(m_RelativePath);
        }

        public string[] QuerySubDirs(string path, bool recursive)
        {
            var result = m_StorageService.QuerySubDirs(m_RelativePath, recursive);
            return result;
        }

        public string[] QuerySubDirs(string path, bool recursive, DirectoryView enDirectoryView)
        {
            var result = m_StorageService.QuerySubDirs(m_RelativePath, recursive, enDirectoryView);
            return result;
        }

        public byte[] ReadStorage(string fileName)
        {
            string filePath = GetRelativeFilePath(fileName);
            var result = m_StorageService.ReadStorage(filePath);
            return result;
        }

        public byte[] ReadPartialStorage(string fileName, long offset, int length) 
        {
            string filePath = GetRelativeFilePath(fileName);
            var result = m_StorageService.ReadPartialStorage(filePath, offset, length);

            return result;
        }

        public void WriteStorage(string fileName, byte[] data)
        {
            string filePath = GetRelativeFilePath(fileName);
            m_StorageService.WriteStorage(filePath, data);
        }

        public void WritePartialStorage(string fileName, long offset, byte[] data) 
        {
            string filePath = GetRelativeFilePath(fileName);
            m_StorageService.WritePartialStorage(filePath, offset, data);
        }

        public void WriteChunkedStorage(string fileName, Queue<MemoryDataChunk> memoryDataChunks)
        {
            string filePath = GetRelativeFilePath(fileName);
            m_StorageService.WriteChunkedStorage(filePath, memoryDataChunks);
        }

        public System.IO.Stream ReadStream(string fileName)
        {
            string filePath = GetRelativeFilePath(fileName);
            return m_StorageService.ReadStream(filePath);
        }

        public void DeleteStorage(string[] fileNames)
        {
            var filesPaths = (from fileName in fileNames
                              select (GetRelativeFilePath(fileName))).ToArray<string>();

            m_StorageService.DeleteStorage(filesPaths);
        }

        public void DeleteFile(string fileName)
        {
            string filePath = GetRelativeFilePath(fileName);
            m_StorageService.DeleteFile(filePath);
        }

        public void RenameStorage(string fileName, string newFileName)
        {
            string filePath = GetRelativeFilePath(fileName);
            string newFilePath = GetRelativeFilePath(fileName);
            m_StorageService.RenameStorage(filePath, newFilePath);
        }

        public bool IsStorageExist(string fileName)
        {
            string filePath = GetRelativeFilePath(fileName);
            return m_StorageService.IsStorageExist(filePath);
        }

        public string[] QueryStorage(string path, string filter)
        {
            return m_StorageService.QueryStorage(m_RelativePath, filter);
        }

        public StorageInfo[] QueryStorageInfo(string path, string sFilter, StorageInfoFlags dataFlags, bool withSubDirs = false)
        {
            return m_StorageService.QueryStorageInfo(m_RelativePath, sFilter, dataFlags, withSubDirs);
        }

        public StorageInfo GetStorageInfo(string fileName, StorageInfoFlags dataFlags)
        {
            string filePath = GetRelativeFilePath(fileName);
            return m_StorageService.GetStorageInfo(filePath, dataFlags);
        }

        #endregion

        #region IStorageCommon Members

		public void WriteDataSet(System.Data.DataSet data, string fileName, bool includeSchema = false)
        {
            string filePath = GetRelativeFilePath(fileName);
			m_StorageService.WriteDataSet(data, filePath, includeSchema);
        }

        #endregion

        private string GetRelativeFilePath(string fileName)
        {
            var result = System.IO.Path.Combine(m_RelativePath, fileName);
            return result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPersistentTransactable Members

        public void PrepareTransaction()
        {
            
        }

        public void ExecuteTransaction()
        {
            
        }

        public void RollbackTransaction()
        {
            
        }

        public bool InTransaction
        {
            get
            {
                return false;
            }
        }

        #endregion

    }
}
