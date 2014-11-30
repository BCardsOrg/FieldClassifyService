using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Data;

namespace TiS.Core.TisCommon.Storage
{
    public enum DirectoryView { Flat, Tree };

   [ServiceContract(Namespace = "http://www.topimagesystems.com/Core/TisCommon/Storage/StorageService")]
    public interface IStorageService : IStorageCommon
    {

        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="path">Directory path</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteDir(string path);

        /// <summary>
        /// Create directory
        /// </summary>
        /// <param name="path">Directory path</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void CreateDir(string path);

        /// <summary>
        /// Gets subdirectories of the current directory
        /// </summary>
        /// <param name="path">Path to storage</param>
        /// <param name="bRecursive">If "true", search will be recursive</param>
        /// <returns>Array of full subdirectories names.</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        string[] QuerySubDirs(string path, bool recursive);

        /// <summary>
        /// Gets subdirectories of the current directory
        /// </summary>
        /// <param name="path">Path to storage</param>
        /// <param name="bRecursive">If "true", search will be recursive</param>
        /// <param name="enDirectoryView">Indicates whether resulting array will contain full path or subdirectory names only.</param>
        /// <returns>Subdirectories array</returns>
        [OperationContract(Name = "QuerySubDirsDirectoryView")]
        string[] QuerySubDirs(string path, bool recursive, DirectoryView enDirectoryView);

        /// <summary>
        /// Reads buffered data
        /// </summary>
        /// <param name="fileName">Storage file name</param>
        /// <returns>Byte array containing the data.</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        byte[] ReadStorage(string fileName);

        /// <summary>
        /// Reads chunk of data from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        byte[] ReadPartialStorage(string fileName, long offset, int length);

        /// <summary>
        /// Writes buffered data
        /// </summary>
        /// <param name="fileName">Name of the file to be written</param>
        /// <param name="data">File contents</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void WriteStorage(string fileName, byte[] data);

        /// <summary>
        /// Writes chunk of data to file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void WritePartialStorage(string fileName, long offset, byte[] data);

        [OperationContract]
        void WriteChunkedStorage(string fileName, Queue<MemoryDataChunk> memoryDataChunks);

        /// <summary>
        /// Opens stream for reading data
        /// </summary>
        /// <param name="fileName">Name of the storage file.</param>
        /// <returns>Read stream</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Stream ReadStream(string fileName);

        /// <summary>
        /// Deletes files
        /// </summary>
        /// <param name="fileNames">File names array</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteStorage(string[] fileNames);

        /// <summary>
        /// Deletes specific file
        /// </summary>
        /// <param name="fileNames">File name</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteFile(string fileName);

       /// <summary>
       /// Renames storage
       /// </summary>
       /// <param name="fileName">Original  file name</param>
       /// <param name="newFileName">New file name</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void RenameStorage(string fileName, string newFileName);

        // Info
        [OperationContract]
        bool IsStorageExist(string fileName);

        /// Requests filtered file list
        /// </summary>
        /// <param name="path">Path to storage</param>
        /// <param name="filter">File filter</param>
        /// <returns>File names array</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        string[] QueryStorage(string path, string filter);

        /// <summary>
        /// Requests filtered storage info for a specific path
        /// </summary>
        /// <param name="path">Path to storage</param>
        /// <param name="sFilter">Request filter</param>
        /// <param name="dataFlags">Requested file info</param>
        /// <returns>Storage info array</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        StorageInfo[] QueryStorageInfo(string path, string sFilter, StorageInfoFlags dataFlags, bool withSubDirs = false);

        /// <summary>
        /// Requests storage info for a specific file
        /// </summary>
        /// <param name="fileName">Storage file name</param>
        /// <param name="dataFlags">Requested storage info flags</param>
        /// <returns></returns>
        /// <summary>
        [OperationContract]
        StorageInfo GetStorageInfo(string fileName, StorageInfoFlags dataFlags);
    }

}
