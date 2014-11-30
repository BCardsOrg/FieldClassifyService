using System;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using log4net.Util;
using log4net.Appender;
using log4net.Core;
using TiS.Logger;

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
        public Log.Severity loggingLevel;
        public string message;
        public Object obj;

        public logItem(Log.Severity Level, string mssg,  Object obj)
        {
            this.loggingLevel = Level;
            this.message = mssg;
            this.obj = obj;
        }
    }

    #region TISAppender

    //Appender that work with MMF..
    public class TISAppender :  AppenderSkeleton
    {
        private TIS_SMData mmfData;     
        private TISSharedMemory<byte> TIS_MMF_INDX; //MMF Data general information
        private TISSharedMemory<byte> TIS_MMF;      //Data Logger  
        private TISLogConfiguration getCfgData;
        private string filePath;

        #region AppenderSkeleton Members

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            try
            {
                // the logger initialize this class automatically
                mmfData = new TIS_SMData();
                TIS_MMF = new TISSharedMemory<byte>("TIS_MMF", (int)TIS_APPENDER.MMFSIZE);
                getCfgData = new TISLogConfiguration();

                TIS_MMF.OutPutFile = Path.Combine(filePath, getCfgData.LogFileName);

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
                    throw new Exception("Failed to Create MMF object]");

            }
            catch
            {
                TIS_MMF = null;
                TIS_MMF_INDX = null;
            }
        }

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

        protected override void OnClose()
        {
            base.OnClose();

            TIS_MMF_INDX.flushData();
            TIS_MMF.flushData();
        }

        #endregion

        ~TISAppender()
       {
           if (TIS_MMF != null)
            TIS_MMF.Close();
           if (TIS_MMF_INDX != null)
            TIS_MMF_INDX.Close();
       }

        // Set from TisLogger.config
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

        #region TISSharedMemory

        public class TISSharedMemory<T> where T : struct
        {
            private bool enableOff;
            private string smName;
            private string mutexName;
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
                get { return fileName; }
                set
                {
                    try
                    {
                        fileName = value;
                    }
                    catch
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
                mutexName = name + "_Mutex";
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

                try
                {
                    fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
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
                if (accessor != null)
                    accessor.Dispose();
                if (TISMmf != null)
                    TISMmf.Dispose();
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

        #endregion
    }

    #endregion
}




