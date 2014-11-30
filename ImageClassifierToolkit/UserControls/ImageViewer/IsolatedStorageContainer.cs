using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{
    public class IsolatedStorageContainer 
    {
        private static Object m_SyncRoot = new Object(); 
        private const long InitialQuota = 524288000; //500MB
        private const decimal QuotaGrowthFactorRelativeToAddedFile = 1.5M;
        private IsolatedStorageFile m_IsolatedStorageFile = null;
        private Dictionary<string, string> m_FileNamesMap = new Dictionary<string, string>();
        private ConcurrentQueue<string> m_FilesToAddToStorageQueue = new ConcurrentQueue<string>();
        private List<string> m_FilesPendingRecycle = new List<string>();
        private bool m_Disposed = false; //used for removing the Isolated Storage File on exit of application

        public IsolatedStorageContainer()
        {
            InitIsolatedStorageFile();            
        }
        
        private void InitIsolatedStorageFile()
        {
            m_IsolatedStorageFile = GetStorageFile();
            //m_IsolatedStorageFile.Quota = InitialQuota; is readonly...
        }

        public void AddFileToStoragePending(string filename)
        {
            m_FilesToAddToStorageQueue.Enqueue(filename);            
        }

        public bool HasQueuedFilesToAdd
        {
            get
            {
                return m_FilesToAddToStorageQueue.Count > 0;
            }
        }

        private void ProcessOneQueuedFileAddToStorage()
        {
            string filename = string.Empty;
            if (m_FilesToAddToStorageQueue.TryDequeue(out filename) == true)
            {
                TryAddFile(filename);
            }
        }
        
        public void Clear()
        {     
            InitIsolatedStorageFile();
            m_IsolatedStorageFile.Remove(); 
            m_FilesToAddToStorageQueue = new ConcurrentQueue<string>();
            m_FileNamesMap.Clear();                             
            m_FilesPendingRecycle.Clear();
        }

        public void Recycle()
        {
            foreach (string file in m_FilesPendingRecycle)
            {
                m_IsolatedStorageFile.DeleteFile(file);
                m_FileNamesMap.Remove(file);                
            }
            m_FilesPendingRecycle.Clear();
            m_FilesToAddToStorageQueue = new ConcurrentQueue<string>();            
        }

        public void MarkCurrenstFileSetAsRecycleable()
        {            
            foreach(string s in m_FileNamesMap.Keys)
            {
                m_FilesPendingRecycle.Add(s);
            }
        }

        private static IsolatedStorageFile GetStorageFile()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        }        

        public Stream GetAddFile(string filename)
        {            
            Stream stream = null;
            string mappedFilename = string.Empty;
            if (m_FileNamesMap.TryGetValue(filename, out mappedFilename) == true)
            {
                lock (m_SyncRoot)
                {
                    stream = m_IsolatedStorageFile.OpenFile(mappedFilename, FileMode.Open, FileAccess.Read, FileShare.Read); //if the file is already taken an exception should occur, and this is fine.
                    
                }
            }
            else
            {
                stream = AddFile(filename);
            }

            return stream; 
            
        }


        private void TryAddFile(string filename)
        {
			if (!m_FileNamesMap.ContainsKey(filename) && System.IO.File.Exists(filename) == true)
            {
                using (Stream stream = AddFile(filename))
                {
                    //just so the resources of the stream are released
                }
            }             
        }

        private Stream AddFile(string filename)
        {
            Stream writeStream = null;
            string guid = Guid.NewGuid().ToString();

            lock (m_SyncRoot)
            {                
                m_FileNamesMap.Add(filename, guid);
                writeStream = m_IsolatedStorageFile.CreateFile(guid);

                using (System.IO.FileStream readStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    GrowQuotaAsNeeded(readStream.Length);
                    readStream.CopyTo(writeStream);
                } 
            }

            return writeStream;
        }

        private void GrowQuotaAsNeeded(long streamLength)
        {
            if (m_IsolatedStorageFile.Quota <= streamLength + m_IsolatedStorageFile.UsedSize)
            {
                if (streamLength < m_IsolatedStorageFile.AvailableFreeSpace)
                {
                    if (m_IsolatedStorageFile.IncreaseQuotaTo((long)(streamLength * QuotaGrowthFactorRelativeToAddedFile)))
                    {

                    }
                    else
                    {
                        throw new IsolatedStorageSpaceNeededExceedsAvailableSpace()
                        {
                            RequiredAdditionalSpace = (long)(streamLength * QuotaGrowthFactorRelativeToAddedFile),
                            AvailableSpace = m_IsolatedStorageFile.AvailableFreeSpace,
                            Quota = m_IsolatedStorageFile.Quota
                        };
                    }
                }
                else
                {
                    throw new IsolatedStorageSpaceNeededExceedsAvailableSpace()
                    {
                        RequiredAdditionalSpace = streamLength,
                        AvailableSpace = m_IsolatedStorageFile.AvailableFreeSpace,
                        Quota = m_IsolatedStorageFile.Quota
                    };
                }

            }
        }

        #region timer
        
        public void StartTimer()
        {
            Timer t = new Timer(new TimerCallback(TimerWake));
            t.Change(0, 500);
        }
       
        private void TimerWake(object obj)
        {
            if (Monitor.TryEnter(m_SyncRoot, 500))
            {
                try
                {
                    Timer timer = obj as Timer;
                    if (HasQueuedFilesToAdd)
                    {
                        ProcessOneQueuedFileAddToStorage();
                    }
                    else if (timer != null)
                    {
                        timer.Dispose();
                        timer = null;
                    }
                }
                finally
                {
                    Monitor.Exit(m_SyncRoot);
                }
            }       
        }
        

        #endregion    
       
    }

    public class IsolatedStorageSpaceNeededExceedsAvailableSpace : Exception
    {
        public long AvailableSpace { get; set; }
        public long Quota { get; set; }
        public long RequiredAdditionalSpace { get; set; }

        public IsolatedStorageSpaceNeededExceedsAvailableSpace() : base()
        {
        }

        public IsolatedStorageSpaceNeededExceedsAvailableSpace(string message) : base(message)
        {
        }

        public IsolatedStorageSpaceNeededExceedsAvailableSpace(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
