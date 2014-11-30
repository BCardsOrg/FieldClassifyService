using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Security.Principal;

using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Configuration;
using log4net;
using log4net.Util;
using log4net.Appender;
using log4net.Core;
using log4net.Repository;


namespace TISAppenderLog4net
{
    /// <summary>
    /// work log4net through Memory-Mapped Files.
    /// </summary>
    /// 
    public enum TIS_APPENDER
    {
        MMFSIZE = 1024*1024,
        MMFMinLvl2Flush = 70000
    }
    public struct TIS_SMData
    {
        public long threadIf;
        public long blockSize;
        public long currentOff;
        public int lastLevel;
        public long totalRecs;
        public bool flushed;
    };

    public struct logItem
    {
        public TiS.Core.TisCommon.Log.Severity loggingLevel;
        public string message;
        public Object obj;

        public logItem(TiS.Core.TisCommon.Log.Severity Level, string mssg,  Object obj)
        {
            this.loggingLevel = Level;
            this.message = mssg;
            this.obj = obj;
        }
    }

    //Appender that work with MMF..
    public class TISAppender : AppenderSkeleton
    {
        private TIS_SMData mmfData;     
        private TISSharedMemory<byte> TIS_MMF_INDX; //MMF Data general information
        private TISSharedMemory<byte> TIS_MMF;      //Data Logger  
        private  LogConfiguration getCfgData;

        public TISAppender()
        {
            try
            {
                // the logger initialize this class automatically
                mmfData = new TIS_SMData();
                TIS_MMF = new TISSharedMemory<byte>("TIS_MMF", (int)TIS_APPENDER.MMFSIZE);
                getCfgData = new LogConfiguration();
                TIS_MMF.OutPutFile = getCfgData.getLogFile();

                if (!TIS_MMF.Open())
                    return;

                byte[] indxBuff = TIS_MMF.StructureToByteArray(mmfData);
                TIS_MMF_INDX = new TISSharedMemory<byte>("TIS_MMF_INDX", indxBuff.Length);

                TIS_MMF_INDX.EnableOffset(false);
                if (TIS_MMF_INDX.Open())
                {
                    indxBuff = TIS_MMF_INDX.ReadArray();
                    TIS_SMData mmfIndex = (TIS_SMData)TIS_MMF_INDX.ByteArrayToStructure(indxBuff);

                    if (mmfIndex.currentOff <= 0)
                        TIS_MMF_INDX.WriteArray(indxBuff);
                    else
                        TIS_MMF.MMFOffset = (int)mmfIndex.currentOff;
                }
                else
                    throw new TisException("Failed to Create MMF object]");

            }
            catch 
            {
                TIS_MMF = null;
                TIS_MMF_INDX = null;
            }
        }

        #region AppenderSkeleton Members

        protected override void Append(LoggingEvent loggingEvent)
        {
            if ((TIS_MMF == null) || (TIS_MMF_INDX == null))
                return;

            LocationInfo lInfo = loggingEvent.LocationInformation;

            // append the logging Event 
            string threadName = loggingEvent.ThreadName;

            byte[] buffer = TIS_MMF.StrToByteArray(getCfgData.getLineFormated(loggingEvent));
            TIS_MMF.WriteArray(loggingEvent.Level.Value,   buffer);

            mmfData.currentOff = TIS_MMF.MMFOffset;         
           TIS_MMF_INDX.WriteArray(TIS_MMF_INDX.StructureToByteArray(mmfData));
        }

       private string GetXmlFormat(string mssg)
        {
           if (mssg.Length <= 0)
            return mssg;
           return string.Empty;
        }

        #endregion
        ~TISAppender()
       {
           if (TIS_MMF != null)
            TIS_MMF.Close();
           if (TIS_MMF_INDX != null)
            TIS_MMF_INDX.Close();
       }
    }

    public class TISEventLogAppender : EventLogAppender
    {
        public TISEventLogAppender()
        {
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            //
            // Write the resulting string to the event log system
            //
            int eventID = 0;

            // Look for the EventLogEventID property
            object eventIDPropertyObj = loggingEvent.LookupProperty("EventID");
            if (eventIDPropertyObj != null)
            {
                if (eventIDPropertyObj is int)
                {
                    eventID = (int)eventIDPropertyObj;
                }
                else
                {
                    string eventIDPropertyString = eventIDPropertyObj as string;
                    if (eventIDPropertyString != null && eventIDPropertyString.Length > 0)
                    {
                        // Read the string property into a number
                        int intVal;
                        if (SystemInfo.TryParse(eventIDPropertyString, out intVal))
                        {
                            eventID = intVal;
                        }
                        else
                        {
                            ErrorHandler.Error("Unable to parse event ID property [" + eventIDPropertyString + "].");
                        }
                    }
                }
            }

            // Write to the event log
            try
            {
                string eventTxt = RenderLoggingEvent(loggingEvent);

                // There is a limit of 32K characters for an event log message
                if (eventTxt.Length > 32000)
                {
                    eventTxt = eventTxt.Substring(0, 32000);
                }

                EventLogEntryType entryType = GetEntryType(loggingEvent.Level);

                using (SecurityContext.Impersonate(this))
                {
                    EventLogPermission eventLogPermission = new EventLogPermission(EventLogPermissionAccess.Administer, ".");

                    eventLogPermission.PermitOnly();

                    EventLog.WriteEntry(ApplicationName, eventTxt, entryType, eventID);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Unable to write to event log [" + LogName + "] using source [" + ApplicationName + "]", ex);
            }

        }
       
    }


    //Appender that work with single Queue and flush to single log file.
    public class TISQueueAppender : AppenderSkeleton
    {
        private string outputFile=string.Empty;
        private FileStream fileS = null;
        private static Mutex TISMutex;
        public int m_MaxFileSize;

        public TISQueueAppender()
        {
            try
            {           
                TISMutex = new Mutex();

                LogConfiguration getCfgData = new LogConfiguration();
                outputFile = getCfgData.getLogFile();       

                if(File.Exists(outputFile))
                    fileS = new FileStream(outputFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                else
                    fileS = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

            }
            catch (Exception exc)
            {
                throw new TisException(exc, "Failed to Create/Open Log File [{0}]", outputFile);
            }
        }

       
        #region AppenderSkeleton Members

        protected override void Append(LoggingEvent loggingEvent)
        {
            TISMutex.WaitOne();
            try
            {
                string tmpStr = RenderLoggingEvent(loggingEvent);
                tmpStr = tmpStr.Replace("&", " "); // Remove invalid characters
                byte[] buffer = StrToByteArray(tmpStr);
                fileS.Write(buffer, 0, buffer.Length-1);
                fileS.Flush();
            }
            catch (Exception exc)
            {
                throw new TisException(exc, "No Log File Was open!");
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
        public string ReadRegKey(string SubKey, string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = Registry.LocalMachine;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(SubKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return string.Empty;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {

                    return string.Empty;
                }
            }
        }

        #endregion
        ~TISQueueAppender()
        {
            fileS.Close();
        }
    }

    //Tools Classes
    internal class TIS_LogFile
    {
        private string path;
        private bool append;
        private FileAccess access;
        private FileShare share;
        private static TIS_LogFile instance;
        private static Mutex TISMutex;
        private static FileStream fileS = null;

        public static TIS_LogFile GetInstance(string path, bool append, FileAccess access, FileShare share)
        {
            if (instance == null)
            {
                instance = new TIS_LogFile(path, append, access, share);
            }

            return instance;
        }

        private TIS_LogFile(string path, bool append, FileAccess access, FileShare share)
        {
            TISMutex = new Mutex();
            this.path = path;
            this.append = append;
            this.access = access;
            this.share = share;

            fileS = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            fileS.Seek(0, SeekOrigin.End);

        }
        
        public int  writeBuff(string buffer)
        {
            if (fileS == null)
                return -1;

            TISMutex.WaitOne();

            byte[] bBuffer = new UTF8Encoding(true).GetBytes(buffer);
            fileS.Write(bBuffer, 0, bBuffer.Length);
            fileS.Flush();
            Thread.Sleep(1);

            TISMutex.ReleaseMutex();
            return 0;
        }

        public void Close()
        {

        }
    }

    public class ConcurrentMinimalLock : FileAppender.MinimalLock
    {
        private string m_filename;
        private bool m_append;
        private Stream m_stream = null;
        private ConcurrentStream c_stream = null;
        public override void OpenFile(string filename, bool append, Encoding encoding)
        {
            m_filename = filename;
            m_append = append;
        }
        public override void CloseFile()
        {
            // NOP
        }
        public override Stream AcquireLock()
        {
            if (m_stream == null)
            {
                try
                {
                    using (CurrentAppender.SecurityContext.Impersonate(this))
                    {
                        string directoryFullName = Path.GetDirectoryName(m_filename);
                        if (!Directory.Exists(directoryFullName))
                        {
                            Directory.CreateDirectory(directoryFullName);
                        }

                        if (c_stream == null)
                        {
                            c_stream = ConcurrentStream.GetInstance(m_filename, m_append, FileAccess.Write, FileShare.Read);
                            c_stream.IsOpen = true;
                        }
                        m_stream = c_stream;
                        m_append = true;
                    }
                }
                catch (Exception e1)
                {
                    CurrentAppender.ErrorHandler.Error("Unable to acquire lock on file " + m_filename + ". " + e1.Message);
                }
            }
            return m_stream;
        }
        public override void ReleaseLock()
        {
            using (CurrentAppender.SecurityContext.Impersonate(this))
            {
                m_stream.Close();
                m_stream = null;
            }
        }
    }

    public class ConcurrentStream : Stream
    {
        private string path;
        private bool append;
        private FileAccess access;
        private FileShare share;
        //base address of our buffer
        IntPtr m_base = IntPtr.Zero;
        private QueueManager queueManager;
        private static ConcurrentStream instance;

        private bool m_isOpen = false;
        public bool IsOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }
        public override bool CanRead
        {
            get { return false; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }
        private long m_length = 0;
        public override long Length 
		{
            get
            {

                if (m_length == 0)
                {
                    m_length = 4;
                    long temp = 0;
                    temp = this.ReadByte();
                    temp <<= 8;
                    temp += this.ReadByte();
                    temp <<= 8;
                    temp += this.ReadByte();
                    temp <<= 8;
                    temp += this.ReadByte();
                    m_length = temp;
                }
                return m_length;
            }
        }
        private long m_position = 0;
        public override long Position
        {
            get { return m_position; }
            set
            {
                if (value < this.Length)
                    m_position = value;
                else
                    m_position = -1;
            }
        }
        public override void Flush()
        {
            if (!IsOpen)
                return;
           
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            IAsyncResult ret = base.BeginRead(buffer, offset, count, callback, state);
            return ret;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            IAsyncResult ret = base.BeginWrite(buffer, offset, count, callback, state);
            return ret;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int i;
            long InternalLength = Length;
            try
            {
                if (!IsOpen)
                    return -1;

                if (buffer.Length - offset < count)
                    throw new ArgumentException("Invalid Offset!");

                Marshal.Copy((IntPtr)(m_base.ToInt64() + m_position), buffer, 0, count);
                m_position += count;
                return count;
            }
            catch (Exception Err)
            {
                throw Err;
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            CachedEntry entry = new CachedEntry(buffer, offset, count);
            queueManager.Enqueue(entry);
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return -1;
        }
        public override void SetLength(long value)
        {
            // not supported!
        }
        public static ConcurrentStream GetInstance(string path, bool append, FileAccess access, FileShare share)
        {
            
            if (instance == null)
            {
                instance = new ConcurrentStream(path, append, access, share);
            }
            return instance;
        }
        private ConcurrentStream(string path, bool append, FileAccess access, FileShare share)
        {
            this.path = path;
            this.append = append;
            this.access = access;
            this.share = share;
            this.queueManager = QueueManager.GetInstance(path, append, access, share);
        }
    }

    internal class QueueManager
    {

        private string path;
        private bool append;
        private FileAccess access;
        private FileShare share;
        private Queue syncQueue = Queue.Synchronized(new Queue());
        private bool running = false;
        private Random rnd = new Random();
        private DateTime retryTime = DateTime.MaxValue;
        private static TimeSpan RETRY_MAX_SPAN = TimeSpan.FromMinutes(1);
        private static QueueManager instance;
        private const int MAX_BATCH_SIZE = 100;

        public static QueueManager GetInstance(string path, bool append, FileAccess access,FileShare share)
        {
            if (instance == null)
            {
                instance = new QueueManager(path, append, access, share);
            }
            return instance;
        }

        private QueueManager(string path, bool append, FileAccess access, FileShare share)
        {
            this.path = path;
            this.append = append;
            this.access = access;
            this.share = share;
        }

        internal void Enqueue(CachedEntry entry)
        {
            syncQueue.Enqueue(entry);

            if (!running)
            {
                lock (this)
                {
                    running = true;
                    Thread th = new Thread(new ThreadStart(this.Dequeue));
                    th.Start();
                }
            }
        }

        public void Dequeue()
        {
            CachedEntry entry = null;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Append, access, share))
                {
                    int processedCount = 0;
                    while (true)
                    {
                        processedCount++;
                        if (syncQueue.Count == 0)
                        {
                            //quit when queue is empty
                            lock (this)
                            {
                                running = false;
                                return;
                            }
                        }
                        else
                        {
                            entry = (CachedEntry)syncQueue.Dequeue();
                        }

                        if (entry != null)
                        {
                            Write(entry, fs);
                        }
                    }
                }
            }
            catch (IOException ioe)
            {
                if (DateTime.Now - retryTime > RETRY_MAX_SPAN)
                {
                    lock (this)
                    {
                        running = false;
                    }
                    throw;
                }
                //When can't aquire lock
                //Wait random time then retry
                Thread.Sleep(rnd.Next(1000));
                Console.WriteLine("Retry:" + DateTime.Now);
                retryTime = DateTime.Now;
                Dequeue();
            }
        }
        private void Write(CachedEntry entry, FileStream fs)
        {
            fs.Write(entry.Buffer, entry.Offset, entry.Count);
            fs.Flush();
        }
    }
    internal class CachedEntry
    {
        private byte[] buffer;
        private int offset;
        private int count;
        internal byte[] Buffer
        {
            get { return buffer; }
        }
        internal int Offset
        {
            get { return offset; }
        }
        internal int Count
        {
            get { return count; }
        }
        internal CachedEntry(byte[] buffer, int offset, int count)
        {
            this.buffer = new byte[buffer.Length];
            buffer.CopyTo(this.buffer, 0);
            this.offset = offset;
            this.count = count;
        }
    }

    public class TISSharedMemory<T> where T : struct
    {
        private bool enableOff;
        private string smName;
        private string mutexName;
        private Mutex mutex;
        private Mutex indxMutex;
        MutexSecurity mutexsecurity;
        private bool requestInitialOwnership;
        private bool mutexCreated;
        private int smSize;
        public bool flusshed;
        private MemoryMappedFile TISMmf;
        private MemoryMappedViewAccessor accessor;

        private int mmfOffset;
        public int MMFOffset
        {
            get { return mmfOffset; }
            set
            {
                if (enableOff)
                    mmfOffset = value;
                else
                    mmfOffset = 0;
            }

        }

        private string fileName;
        public string OutPutFile
        {
            get { return fileName;}
            set 
            {
                try
                {
                    fileName = value;
                }
                catch (Exception e)
                {
                    
                }
                
            }
        }
        
        public TISSharedMemory(string name, int size)
        {
            flusshed = false;
            mmfOffset = 0;
            smName = name;
            smSize = size;        
            mutexName = name+"_Mutex";
            enableOff = true;
        }

        #region Methods

        public void EnableOffset(bool status)
        {
            enableOff = status;
        }

        public bool Open()
        {
            try
            {
                requestInitialOwnership = false;
                //createMutex();              
                //mutex = new Mutex(requestInitialOwnership, mutexName, out mutexCreated, mutexsecurity);

                TISMmf = MemoryMappedFile.CreateOrOpen(smName, smSize);
                accessor = TISMmf.CreateViewAccessor(0, smSize);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool OpenExisting()
        {
            try
            {
                requestInitialOwnership = false;
                TISMmf = MemoryMappedFile.OpenExisting(smName);
                accessor = TISMmf.CreateViewAccessor(0, smSize);
            }
            catch 
            {
                return false;
            }
            return true;
        }

        public bool CreateFromFile(string file)
        {

            try
            {
                requestInitialOwnership = false;
               // mutex = new Mutex(requestInitialOwnership, mutexName, out mutexCreated);

                FileInfo fi = new FileInfo(file);

                TISMmf = MemoryMappedFile.CreateFromFile(file, FileMode.OpenOrCreate);
                accessor = TISMmf.CreateViewAccessor();
            }
            catch 
            {
                return false;
            }
            return true;
        }


        // Write a buffer to MMF object
        public void WriteArray(byte[] buffer)
        {
            if (mmfOffset + buffer.Length > smSize)
            {
                flushData();
                MMFOffset = 0;
            }

            createMutex();
            using (Mutex m = new Mutex(requestInitialOwnership, mutexName, out mutexCreated, mutexsecurity))
            {
                m.WaitOne();
                accessor.WriteArray<byte>(mmfOffset, buffer, 0, buffer.Length - 1);
                MMFOffset += buffer.Length - 1;
                m.ReleaseMutex();
            }
        }     

        public void WriteArray(int level, byte[] buffer)
        {
            WriteArray(buffer);

            if (level >= (int)TIS_APPENDER.MMFMinLvl2Flush)
            {
                flushData();
                //MMFOffset = 0;
            };
        }
        
        // Read a buffer from mmf object
        public byte[] ReadArray()
        {
            byte[] buffer = ReadArray(MMFOffset, smSize);
            if (buffer == null)
                return buffer;

            if (MMFOffset + buffer.Length > smSize)
                MMFOffset = 0;
            else
                MMFOffset += buffer.Length;
            return buffer;      
        }

        public byte[] ReadArray(int offset, int size)
        {
            byte[] buffer = new byte[size];
            createMutex();
            using (Mutex m = new Mutex(requestInitialOwnership, mutexName, out mutexCreated, mutexsecurity))
            {

                m.WaitOne();
                accessor.ReadArray<byte>(offset, buffer, 0, size);
                m.ReleaseMutex();
            }
            return buffer;
        }

        public bool OpenFile()
        {
            return false;
        }
        public void flushData()
        {
            if (!enableOff)
                return;

            FileStream fs = null;
            BinaryWriter bw = null;
            FileMode fMode = FileMode.OpenOrCreate;

            try
            {           
                fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                bw = new BinaryWriter(fs);
            }
            catch
            {
                return;
            }

            byte[] buffer = new byte[mmfOffset];
            buffer = ReadArray(0, mmfOffset - 1);

            if (buffer.Length > 0)
            {
                bw.Write(buffer);
            }
            flusshed = true;
            bw.Close();
        }

        public void Close()
        {
            if (accessor!=null)
                accessor.Dispose();
            if (TISMmf != null)
                TISMmf.Dispose();
            if (mutex != null)
                mutex.Close();
        }

        public byte[] StrToByteArray(string str)
        { 
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding(); 
            return encoding.GetBytes(str); 
        }

        public byte[] StructureToByteArray(object obj)
        {       
            int Length = Marshal.SizeOf(obj);
            byte[] bytearray = new byte[Length];
            IntPtr ptr = Marshal.AllocHGlobal(Length);
            Marshal.StructureToPtr(obj, ptr, false);
            Marshal.Copy(ptr, bytearray, 0, Length);
            Marshal.FreeHGlobal(ptr);
            return bytearray;
        }
        public object ByteArrayToStructure(byte[] bytearray)
        {
            IntPtr ptr = IntPtr.Zero;

            try
            {
                int Length = bytearray.Length;
                ptr = Marshal.AllocHGlobal(Length);
                Marshal.Copy(bytearray, 0, ptr, Length);
                return Marshal.PtrToStructure(ptr, typeof(TIS_SMData));
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }

        }

        private void createMutex()
        {
            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            mutexsecurity = new MutexSecurity();
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.ChangePermissions, AccessControlType.Deny));
            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.Delete, AccessControlType.Deny));
        }

        #endregion
    }

    public class LogConfiguration : GlobalConfigurationService
    {
        private const string SECTION_NAME = "log4net";
        public const string LOG_FILES_LOCATION = "Log";
        public const string LOGGER_CONFIG_FILE_NAME = "TISLogger.xml";

        public string GetLogFilesPath()
        {

            string logFilePath = GetConfigFilesPath();

            if (String.IsNullOrEmpty(logFilePath))
            {
                throw new TisException("Failed to load path to configuration file from registry ({0}\\{1})", GlobalConfigurationService.REG_CONFIG_FILES_ENTRY, LOG_FILES_LOCATION);
            }

            int indx = logFilePath.IndexOf(CONFIG_SUBDIR);
            return string.Format("{0}{1}", logFilePath.Remove(indx), LOG_FILES_LOCATION);
        }

        public string GetCnfgFile()
        {
            string res = string.Empty;
            try
            {
                res = Path.Combine(base.GetConfigFilesPath(), LOGGER_CONFIG_FILE_NAME);
            }
            catch
            {
                return null;
            }
            return res;
        }

        public string getLogFile()
        {
            string sufix = string.Format("{0}.{1}.{2}", System.DateTime.Now.Day, System.DateTime.Now.Month, System.DateTime.Now.Year);
            string LogPath = GetConfigFilesPath();
            LogPath = string.Format("{0}\\Log", LogPath.Remove(LogPath.LastIndexOf("\\")));

            try
            {
                if (!Directory.Exists(LogPath))
                    System.IO.Directory.CreateDirectory(LogPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return string.Format("{0}\\{1}_{2}.xml", LogPath, "TISLog", sufix); 
        }

        public string getLogDefPath()
        {
            string LogPath = GetConfigFilesPath();
            LogPath = string.Format("{0}\\Log", LogPath.Remove(LogPath.LastIndexOf("\\")));
            return LogPath;
        }

        public string getLineFormated(LoggingEvent loggingEvent)
        {
            try 
            {                
                return string.Format("<log4j:event logger=\"{0}\" timestamp=\"{1}\" level=\"{2}\" thread=\"{3}\"><log4j:message>{4}</log4j:message>" +
                                          "<log4j:properties><log4j:data name=\"log4net:UserName\" value=\"{5}\" /><log4j:data name=\"log4jmachinename\" value=\"{6}\" />" +
                                          "<log4j:data name=\"log4japp\" value=\"{7}\" /><log4j:data name=\"log4net:HostName\" value=\"{8}\" />" +
                                          "<log4j:throwable><![CDATA[{11}]]></log4j:throwable> />" +
                                          "</log4j:properties><log4j:locationInfo class=\"{9}\" method=\"{10}\" ErrorType=\"{8}\" /></log4j:event>\r\n",
                                           loggingEvent.LoggerName,                                 // Logger       [0]
                                           loggingEvent.TimeStamp.ToUniversalTime().Ticks,                   // TimeStamp    [1] 
                                           loggingEvent.Level,                                      // Level        [2]
                                           loggingEvent.ThreadName,                                 // Thread       [3]
                                           loggingEvent.RenderedMessage,                            // Message      [4]
                                           loggingEvent.UserName,                                   // User name    [5]
                                           loggingEvent.ThreadName,                                 // Machine Name [6]
                                           loggingEvent.GetExceptionString(),                       // Data         [7]
                                           loggingEvent.Identity,                                   // Host         [8]
                                           loggingEvent.LocationInformation.ClassName,              // Class        [9]
                                           loggingEvent.LocationInformation.MethodName,             // Method       [10]
                                           loggingEvent.Domain.ToString()                           // Throwable    [11]                
                                        );
                
            }
            catch
            {
                return string.Empty;
            }
        }

        public class Log4NetException : System.Exception
        {
            public Log4NetException() : base() { TISMessage = string.Empty; m_StackTrace = string.Empty; }
            public Log4NetException(string message) : base(message) { TISMessage = message; m_StackTrace = string.Empty; }
            public Log4NetException(string message, System.Exception inner) : base(inner.Message, inner)
            { 
                TISMessage = message;
                this.m_StackTrace =  inner.StackTrace;
            }

            public Log4NetException(string message, System.Exception inner, StackFrame frame): base(inner.Message, inner)
            {
                TISMessage = message;
                m_StackTrace = inner.StackTrace;
                MethodBase method = frame.GetMethod();
                Source = method.Module.Name;
                m_Class = method.DeclaringType.ToString();
                m_Method = method.Name;
            }

            public string TISMessage;
            public string m_Class;
            public string m_Method;
            public string m_StackTrace;

            // A constructor is needed for serialization when an
            // exception propagates from a remoting server to the client. 
            protected Log4NetException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) { }
        }


    }
 


}




