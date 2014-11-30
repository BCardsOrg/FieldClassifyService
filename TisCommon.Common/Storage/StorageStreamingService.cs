using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.Storage
{
	[ComVisible(false)]
	public interface IStorageStreamingService
	{
		void ToStream(
			Stream targetStream,
			IStorageService storage,
			string path,
			string filter,
			bool includeSubDirs);

		void ToStream(
			Stream targetStream,
			IStorageService blobStorage,
			string path,
			string filter,
			bool includeSubDirs,
			DirectoryView directoryView);

		void ToStream(
			Stream targetStream,
			IStorageService storage,
			string path,
			string filter,
			QueryBlobsFilterDelegate queryBlobsFilter,
			bool includeSubDirs,
			DirectoryView directoryView);

        void FromStream(
            Stream sourceStream,
            IStorageService storage);

		void FromStream(
			Stream sourceStream,
			IStorageService storage,
			QueryBlobsReadFilterDelegate queryBlobsFilter);
	}

	public delegate void QueryBlobsFilterDelegate(string blobName, ref bool passed);

	public delegate void QueryBlobsReadFilterDelegate(
	   string blobName,
	   long blobSize,
	   DateTime lastWriteTime,
	   ref bool passed);

	public enum Compression { None = 0, Medium = 5, Maximum = 9 }

	[ComVisible(false)]
	public class StorageZipService : IStorageStreamingService
	{
	    private readonly ZipStorer.Compression m_compression = ZipStorer.Compression.Deflate;

		//
		//	Public methods
		//

		public StorageZipService(Compression compression)
		{
		    m_compression = (compression == Compression.None)
		                        ? ZipStorer.Compression.Store
		                        : ZipStorer.Compression.Deflate;
		}

		public StorageZipService()
		{
		}

		public void ToStream(
			Stream targetStream,
			IStorageService storage,
			string path,
			string filter,
			QueryBlobsFilterDelegate queryFilter,
			bool includeSubDirs,
			DirectoryView directoryView)
		{
			// Root is always included
			var directories = new List<string> {path};

			if (includeSubDirs)
			{
				directories.AddRange(storage.QuerySubDirs(
					path,
					true, // Include subdirs
					directoryView));
			}

            using (var zipStorer = ZipStorer.Create(targetStream))
            {
                // Handle all subdirs
                foreach (string subDir in directories)
                {
                    // Stream directory
                    DirToZipStream(
                        storage,
                        subDir,
                        filter,
                        queryFilter,
                        zipStorer);
                }
            }
		}

		public void ToStream(
			Stream targetStream,
			IStorageService storage,
			string path,
			string filter,
			bool includeSubDirs)
		{
			ToStream(targetStream, storage, path, filter, null, includeSubDirs, DirectoryView.Tree);
		}

		public void ToStream(
			Stream targetStream,
			IStorageService storage,
			string path,
			string filter,
			bool includeSubDirs,
			DirectoryView directoryView)
		{
			ToStream(targetStream, storage, path, filter, null, includeSubDirs, directoryView);
		}

		public void FromStream(Stream sourceStream, IStorageService storage)
		{
			FromStream(sourceStream, storage,  null);
		}

		public void FromStream(
			Stream sourceStream,
			IStorageService blobStorage,
			QueryBlobsReadFilterDelegate queryBlobsFilter)
		{
			using (var zipStorer = ZipStorer.Open(sourceStream, FileAccess.Read))
            {
                IEnumerable<ZipStorer.ZipFileEntry> zipEntries = zipStorer.ReadCentralDir();

                if (queryBlobsFilter != null)
                {
                    zipEntries = zipEntries.Where(entry => {
                                            bool passed = true;
                                            queryBlobsFilter(entry.FilenameInZip, entry.FileSize, entry.ModifyTime, ref passed);
                                            return passed;
                                        });
                }

                // Copy data from zip stream to BLOBs
                foreach (var entry in zipEntries)
                {
                    string fileName = Path.GetFileName(entry.FilenameInZip);

                    // Skip if we have no filename
                    if (string.IsNullOrEmpty(fileName)) continue;

                    // Read zip entry and store it as BLOB
                    using (var writeStream = new MemoryStream())
                    {
                        zipStorer.ExtractFile(entry, writeStream);
                        
                        blobStorage.WriteStorage(entry.FilenameInZip, writeStream.ToArray());
                    }
                }
            }			
		}

		//
		//	Private methods
		//

		private void DirToZipStream(
			IStorageService storage,
			string dir,
			string filter,
			QueryBlobsFilterDelegate queryFilter,
			ZipStorer zipStorer)
		{
			IEnumerable<string> storageNames = storage.QueryStorage(dir, filter);

            if (queryFilter != null)
            {
                storageNames = storageNames.
                    Where(name => {
                            bool passed = true;
                            queryFilter(name, ref passed);
                            return passed;
                    });
            }
            
			try
			{
				foreach (string fileName in storageNames.Select(storageName => Path.Combine(dir, storageName)))
				{
				    using (Stream readStream = new MemoryStream(storage.ReadStorage(fileName)))
				    {
				        zipStorer.AddStream(m_compression, fileName, readStream, DateTime.Now);						
				    }
				}				
			}
            catch (Exception ex)
            {
                Log.WriteException(ex);
            }
		}
	}
}
