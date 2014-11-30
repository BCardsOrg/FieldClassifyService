using System;
using System.IO;
using System.Text;
using System.Threading;

namespace TiS.Core.TisCommon
{
	public class FileUtil
	{
		public const int DEFAULT_BUFF_SIZE = 64*1024;

		public static string GetFileAsString(string sFileName)
		{
			using(StreamReader oReader = new StreamReader(sFileName))
			{
				return oReader.ReadToEnd();
			}
		}

		public static void CreateFileFromString(string sFileName, string sContents)
		{
			using(StreamWriter oWriter = new StreamWriter(sFileName))
			{
				oWriter.Write(sContents);
				oWriter.Flush();
			}
		}

        /// <summary>
        /// Return true if file is close  - i.e. file can be open for write
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsFileClosed(string filename)
        {
            try
            {
                using (var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
        
        public static void CreateFileFromByteArray(
			string sFileName,
			byte[] Contents)
		{
            Directory.CreateDirectory(Path.GetDirectoryName(sFileName));

            try
            {
                File.WriteAllBytes(sFileName, Contents);
                return;
            }
            // In case file is download by other process
            catch (System.IO.IOException e)
            {
                // Wait 10sec for the file will to be created...
                for (int i = 0; i < 10000; i += 500)
                {
                    System.Threading.Thread.Sleep(500);
                    if (IsFileClosed(sFileName) == true)
                    {
                        return;
                    }
                }
                throw e;
            }
        }

		public static byte[] GetFileAsByteArray(
			string sFileName)
		{
			return ReadAllBytes(sFileName);
		}

        public static byte[] ReadAllBytes(string path)
        {
            byte[] bytes;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int index = 0;
                long fileLength = fs.Length;

                if (fileLength > Int32.MaxValue)
                {
                    throw new TisException("File [{0}] too long.", path);
                }

                int count = (int)fileLength;
                bytes = new byte[count];

                while (count > 0)
                {
                    int n = fs.Read(bytes, index, count);

                    if (n == 0)
                    {
                        throw new TisException("End of file [{0}] reached before expected.", path);
                    }

                    index += n;
                    count -= n;
                }
            }

            return bytes;
        }

        public static void WriteAllBytes(string path, byte[] data)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
        }

        public static void DeleteDirectory(string path, bool recurcive)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, recurcive);
                }
            }
            catch
            {
                Thread.Sleep(0);

                try
                {
                    Directory.Delete(path, recurcive);
                }
                catch (Exception exc)
                {
                    Log.WriteWarning("Failed to delete client directory [{0}]. Details : {1}",
                        path,
                        exc.Message);
                }
            }
        }
    }
}
