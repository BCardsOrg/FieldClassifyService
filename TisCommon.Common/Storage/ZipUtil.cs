using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Storage
{
	[ComVisible(false)]
	public static class ZipUtil
	{
		private const string DefaultZipEntry = "_DefEntry";
		
		public static byte[] ZipArchive(byte[] data, Compression compression)
		{
            ZipStorer.Compression comp = (compression == Compression.None)
                                                        ? ZipStorer.Compression.Store
                                                        : ZipStorer.Compression.Deflate;

            using (var zipStream = new MemoryStream())
            {
                using (var zipStorer = ZipStorer.Create(zipStream))
                using (var dataStream = new MemoryStream(data))
                {
                    zipStorer.AddStream(comp, DefaultZipEntry, dataStream, DateTime.Now);
                }

                return zipStream.ToArray();
            }
		}

		public static byte[] ZipArchive(Stream stream, Compression compression)
		{
			return ZipArchive(
				StreamUtil.ReadToEnd(stream), 
				compression);
		}

		public static byte[] ZipUnarchive(byte[] data)
		{
			return ZipUnarchive(new MemoryStream(data));
		}

		public static byte[] ZipUnarchive(Stream stream)
		{
			// Create ZipInputStream
            using (var zipStorer = ZipStorer.Open(stream, FileAccess.Read))
            {
                var entries = zipStorer.ReadCentralDir();				
				if (entries.Count == 0)
				{
					throw new TisException("Empty zip stream provided");
				}

                var firstEntry = entries[0];
				if(firstEntry.FilenameInZip != DefaultZipEntry)
				{
                    throw new TisException("Invalid zip entry [{0}], {1} expected", firstEntry.FilenameInZip, DefaultZipEntry);
				}

                byte[] data;
                using (var outputStream = new MemoryStream((int)firstEntry.FileSize))
                {
                    zipStorer.ExtractFile(firstEntry, outputStream);

                    data = outputStream.ToArray();
                }
                
                if (data.Length != firstEntry.FileSize)
				{
					throw new TisException("Failed read zip entry, [{0}] bytes read, {1} expected", data.Length, firstEntry.FileSize);
				}

				return data;
			}
		}
	}
}
