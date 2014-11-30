using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace TiS.Core.TisCommon
{
    public static class ImageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="destinantionImage"></param>
        /// <param name="pageNumbers"></param>
		public static void CopyImageStream(Stream sourceImage, Stream destinantionImage, IEnumerable<int> pageNumbers)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Compression = TiffCompressOption.Ccitt4;
            string tempFile = Path.GetTempFileName();
            TiffBitmapDecoder decoder;
			decoder = new TiffBitmapDecoder(sourceImage, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

			// Get the images we already have in the destination 
			if (destinantionImage.Length > 0)
			{
				TiffBitmapDecoder dstDecoder;
				dstDecoder = new TiffBitmapDecoder(destinantionImage, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
				foreach (var item in dstDecoder.Frames)
				{
					encoder.Frames.Add(item);
				}
			}

			// Get the new images from source
			int frames = decoder.Frames.Count;
            foreach (int pageNumber in pageNumbers)
            {
                if (frames < pageNumber)
                    break;
                encoder.Frames.Add(decoder.Frames[pageNumber-1]);
            }

			// Save all to destination
			encoder.Save(destinantionImage);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinantionFile"></param>
        /// <param name="pageNumbers"></param>
		public static void CopyImageFile(string sourceFile, string destinantionFile, IEnumerable<int> pageNumbers)
		{
			using (var srcImage = File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using( var desImage = File.Open(destinantionFile, FileMode.OpenOrCreate, FileAccess.ReadWrite) )
				{					
					CopyImageStream( srcImage, desImage, pageNumbers );
				}
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pagesToRemove"></param>
        /// <returns></returns>
		public static Stream RemovePagesFromImageStream(Stream image, IEnumerable<int> pagesToRemove)
		{
			MemoryStream write = new MemoryStream();

			Dictionary<int, int> pages = (pagesToRemove == null) ? new Dictionary<int, int>() : pagesToRemove.ToDictionary(p => p);
			TiffBitmapEncoder encoder = new TiffBitmapEncoder();
			encoder.Compression = TiffCompressOption.Ccitt4;
			TiffBitmapDecoder decoder;
			decoder = new TiffBitmapDecoder(image, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);

			int frames = decoder.Frames.Count;
			for (int i = 0; i < frames; i++)
			{
				if (pages.ContainsKey(i) == true)
					continue;
				encoder.Frames.Add(decoder.Frames[i]);

			}

			encoder.Save(write);

			return write;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageFileName"></param>
        /// <param name="pagesToRemove"></param>
		public static void RemovePagesFromImageFile(string imageFileName, IEnumerable<int> pagesToRemove)
        {
			var newImage = RemovePagesFromImageStream(new MemoryStream(FileUtil.ReadAllBytes(imageFileName)), pagesToRemove);
			newImage.Position = 0;
			using(var newImageFile = new FileStream(imageFileName, FileMode.Create))
			{
				const int bufferSize = 32000;
				byte [] memBuffer = new byte[bufferSize];
				while (newImage.Position < newImage.Length)
				{
					int bufferLen = newImage.Read(memBuffer, 0, bufferSize);
					newImageFile.Write(memBuffer, 0, bufferLen);
				}
			}
		}
        /// <summary>
        /// Add a new page to image file
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="pageBitmap"></param>
        public static void AddPageToImageFile(string imageFile, Bitmap pageBitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            using (var desImage = File.Open(imageFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                pageBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                ImageHelper.CopyImageStream(stream, desImage, new int[] { 1 });
            }
        }
        /// <summary>
        /// Swaps pages in tiff file
        /// </summary>
        /// <param name="fileName">Multitiff file name</param>
        /// <param name="sourcePageIndex">Source index</param>
        /// <param name="destinationPageIndex">Destination index</param>
        public static Stream SwapPages(Stream stream, int sourcePageIndex, int destinationPageIndex)
        {
            TiffBitmapDecoder decoder = (TiffBitmapDecoder)TiffBitmapDecoder.Create(
                stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Compression = GetImageCompression(new Bitmap(stream));

            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                if (i == sourcePageIndex)
                {
                    encoder.Frames.Add(decoder.Frames[destinationPageIndex]);
                }
                else if (i == destinationPageIndex)
                {
                    encoder.Frames.Add(decoder.Frames[sourcePageIndex]);
                }
                else
                {
                    encoder.Frames.Add(decoder.Frames[i]);
                }
            }

            MemoryStream imageStream = new MemoryStream();
            encoder.Save(imageStream);

            return imageStream;
        }
        /// <summary>
        /// Rotates specified page in tifffile. Rotate angle should be 0, 90, 180, 270
        /// </summary>
        public static Stream Rotate(Stream stream, int pageIndex, double angle)
        {
            TiffBitmapDecoder decoder = (TiffBitmapDecoder)TiffBitmapDecoder.Create(
                stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Compression = GetImageCompression(new Bitmap(stream));

            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                if (i == pageIndex)
                {
                    encoder.Frames.Add(RotateImage(decoder.Frames[i], angle));
                }
                else
                {
                    encoder.Frames.Add(decoder.Frames[i]);
                }
            }

            MemoryStream imageStream = new MemoryStream();
            encoder.Save(imageStream);

            return imageStream;
        }
        /// <summary>
        /// Merges files to tifffile
        /// </summary>
        public static void MergeFiles(string destinationFileName, IEnumerable<string> filesToMerge)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Compression = GetImageCompression(Image.FromFile(filesToMerge.ElementAt(0)));

            foreach (var file in filesToMerge)
            {
                BitmapDecoder decoder = TiffBitmapDecoder.Create(
                    ReadFile(file), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                for (int i = 0; i < decoder.Frames.Count; i++)
                {
                    encoder.Frames.Add(decoder.Frames[i]);
                }
            }

            MemoryStream memoryStream = new MemoryStream();
            encoder.Save(memoryStream);

            using (Stream stream = File.Open(destinationFileName, FileMode.CreateNew, FileAccess.Write))
            {
                stream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            }
        }

        public static void InsertFile(string destinationFileName, Dictionary<int, string> pageList)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Compression = GetImageCompression(Image.FromFile(destinationFileName));

            for (int i = 0; i < GetPageCount(destinationFileName); i++)
            {
                BitmapDecoder decoder = TiffBitmapDecoder.Create(ReadFile(destinationFileName), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                encoder.Frames.Add(decoder.Frames[i]);
            }

            foreach (var item in pageList)
            {
                BitmapDecoder newDecoder = TiffBitmapDecoder.Create(ReadFile(item.Value), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                int indx = item.Key == 0 ? 0 : item.Key - 1;
                encoder.Frames.Insert(indx, newDecoder.Frames[0]);
            }

            MemoryStream memoryStream = new MemoryStream();
            encoder.Save(memoryStream);

            Stream UpdateFile = File.Open(destinationFileName, FileMode.Create, FileAccess.Write);

            UpdateFile.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            UpdateFile.Close();
            UpdateFile.Dispose();
        }


        /// <summary>
        /// Splits multitiff to jpegs files
        /// </summary>
        public static void SplitFiles(string sourceFileName, IEnumerable<string> destinationFileNames, bool convertToJpeg)
        {
            string extension = convertToJpeg ? ".jpg" : ".prv";
            // Get compression
            var compression = GetImageCompression(Image.FromFile(sourceFileName));

            MemoryStream stream = ReadFile(sourceFileName);

            BitmapDecoder decoder = TiffBitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            for (int i = 0; i < destinationFileNames.Count(); i++)
            {
                MemoryStream outputStream = new MemoryStream();

                BitmapEncoder encoder = CreateBitmapEncoder(true, compression);

                if (i >= decoder.Frames.Count())
                    continue;

                encoder.Frames.Add(decoder.Frames[i]);

                encoder.Save(outputStream);
                string fileName = Path.Combine(Path.GetDirectoryName(destinationFileNames.ElementAt(i)), Path.GetFileNameWithoutExtension(destinationFileNames.ElementAt(i)) + extension);

                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(outputStream.ToArray());
                }
            }

            //destinationFileNames = destinationFileNames.Select(x =>
            //{
            //    x = (x == string.Empty) ? string.Empty : Path.Combine(Path.GetDirectoryName(x), Path.GetFileNameWithoutExtension(x) + extension);
            //    return x;
            //}).ToList();

        }
        /// <summary>
        /// Returns the pages number in given tiff file 
        /// </summary>
        public static int GetPageCount(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
            {
                BitmapDecoder decoder = TiffBitmapDecoder.Create(
                    stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                return decoder.Frames.Count;
            }
        }
        /// <summary>
        /// Copies given frame to the target tiff file
        /// </summary>
        public static void CopyTiffPage(string sourceTiff, int sourcePageIndex, string targetTiff, int targetPageIndex)
        {
            MemoryStream memoryStream = null;

            using (Stream sourceStream = File.OpenRead(sourceTiff))
            {
                BitmapFrame frame = GetTiffPage(sourceStream, sourcePageIndex);

                using (Stream targetStream = File.OpenRead(targetTiff))
                {
                    memoryStream = InsertTiffPage(targetStream, frame, targetPageIndex);
                }
            }

            File.WriteAllBytes(targetTiff, memoryStream.ToArray());
        }
        /// <summary>
        /// Reads Frame by its index
        /// </summary>
        public static BitmapFrame GetTiffPage(Stream stream, int pageIndex)
        {
            TiffBitmapDecoder decoder = new TiffBitmapDecoder(
                stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);

            return decoder.Frames[pageIndex];
        }
        /// <summary>
        /// 
        /// </summary>
        public static MemoryStream InsertTiffPage(Stream stream, BitmapFrame frame, int pageIndex)
        {
            TiffBitmapDecoder decoder = new TiffBitmapDecoder(
                stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);

            MemoryStream targetStream = new MemoryStream();
            TiffBitmapEncoder encoder = new TiffBitmapEncoder() { Compression = TiffCompressOption.Ccitt4 };

            foreach (var current in decoder.Frames)
            {
                encoder.Frames.Add(current);
            }

            encoder.Frames.Insert(pageIndex, frame);

            encoder.Save(targetStream);
            return targetStream;
        }

        #region Private methods

        private static BitmapEncoder CreateBitmapEncoder(bool convertToJpeg, TiffCompressOption compression)
        {
            if (convertToJpeg == true)
            {
                return new JpegBitmapEncoder();
            }
            else
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Compression = compression;

                return encoder;
            }
        }

        private static MemoryStream ReadFile(string file)
        {
            byte[] buffer;
            using (BinaryReader reader = new BinaryReader(File.OpenRead(file)))
            {
                buffer = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            return new MemoryStream(buffer);
        }

        private static BitmapFrame RotateImage(BitmapFrame frame, double angle)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(frame);

            MemoryStream stream = new MemoryStream();
            encoder.Save(stream);

            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = stream;
            imageSource.EndInit();

            CachedBitmap cache = new CachedBitmap(imageSource,
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            TransformedBitmap tb = new TransformedBitmap(cache, new RotateTransform(angle));

            return BitmapFrame.Create(tb);
        }

        private static TiffCompressOption GetImageCompression(Image image)
        {
            int compressionTagIndex = Array.IndexOf(image.PropertyIdList, 0x103);
            if (compressionTagIndex > 0)
            {
                PropertyItem compressionTag = image.PropertyItems[compressionTagIndex];

                return (TiffCompressOption)(BitConverter.ToInt16(compressionTag.Value, 0) - 1);
            }
            else
                return TiffCompressOption.Ccitt3;
        }
        #endregion

    }
}
