using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace TiS.Core.TisCommon
{
    [System.Runtime.InteropServices.ComVisible(false)]
    public class CompressUtil
    {
        public static bool CanCompressString(string s)
        {
            return StringUtil.IsStringInitialized(s);
        }

        public static byte[] CompressString(string s)
        {
            if (CanCompressString(s))
            {
                try
                {
                    using (MemoryStream targetStream = new MemoryStream())
                    {
                        using (GZipStream zipStream = new GZipStream(targetStream, CompressionMode.Compress, true))
                        {
                            byte[] stringBytes = Encoding.UTF8.GetBytes(s);

                            zipStream.Write(stringBytes, 0, stringBytes.Length);
                        }

                        targetStream.Flush();

                        targetStream.Seek(0, SeekOrigin.Begin);

                        return StreamUtil.ReadToEnd(targetStream);
                    }
                }
                catch(Exception exc)
                {
                    throw new TisException(exc, exc.Message);
                }
            }
            else
            {
                throw new TisException("Can not compress empty string.");
            }
        }

        public static bool CanDecompressString(byte[] stringBytes)
        {
            return stringBytes != null && stringBytes.Length > 0;
        }

        public static string DecompressString(byte[] stringBytes)
        {
            if (CanDecompressString(stringBytes))
            {
                try
                {
                    using (MemoryStream sourceStream = new MemoryStream(stringBytes))
                    {
                        byte[] buffer = GetReadBuffer(sourceStream);

                        using (GZipStream zipStream = new GZipStream(sourceStream, CompressionMode.Decompress, false))
                        {
                            int readOffset = 0;

                            while (true)
                            {
                                int bytesRead = zipStream.Read(buffer, readOffset, 100);

                                if (bytesRead == 0)
                                {
                                    break;
                                }

                                readOffset += bytesRead;
                            }
                        }

                        return new String(Encoding.UTF8.GetChars(buffer));
                    }
                }
                catch (Exception exc)
                {
                    throw new TisException(exc, exc.Message);
                }
            }
            else
            {
                throw new TisException("Can not decompress empty string.");
            }
        }

        private static byte[] GetReadBuffer(MemoryStream sourceStream)
        {
            byte[] buffer = new byte[4];

            sourceStream.Position = (int)sourceStream.Length - 4;
            sourceStream.Read(buffer, 0, 4);

            sourceStream.Position = 0;

            int bufferLength = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[bufferLength + 100];

            return buffer;
        }
    }
}
