using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace TISAppenderLog4net
{
    //Appender that work with single Queue and flush to single log file.
    public class TISQueueAppender : AppenderSkeleton
    {
        private string outputFile = string.Empty;
        private FileStream fileS = null;
        private static Mutex TISMutex;
        public int m_MaxFileSize;
        private string filePath;

        #region AppenderSkeleton Members

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            try
            {
                TISMutex = new Mutex();

                TISLogConfiguration getCfgData = new TISLogConfiguration();

                outputFile = Path.Combine(filePath, getCfgData.LogFileName);

                if (File.Exists(outputFile))
                    fileS = new FileStream(outputFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                else
                    fileS = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Failed to Create/Open Log File [{0}]", outputFile), exc);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            TISMutex.WaitOne();
            try
            {
                string tmpStr = RenderLoggingEvent(loggingEvent);
                tmpStr = tmpStr.Replace("&", " "); // Remove invalid characters
                byte[] buffer = StrToByteArray(tmpStr);
                fileS.Write(buffer, 0, buffer.Length - 1);
                fileS.Flush();
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("No Log File Was open!"), exc);
            }
            finally
            {
                TISMutex.ReleaseMutex();
            };
        }

        public byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        #endregion

        ~TISQueueAppender()
        {
            fileS.Close();
        }

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;

                try
                {
                    if (!Directory.Exists(filePath))
                        System.IO.Directory.CreateDirectory(filePath);
                }
                catch
                {
                }
            }
        }
    }
}
