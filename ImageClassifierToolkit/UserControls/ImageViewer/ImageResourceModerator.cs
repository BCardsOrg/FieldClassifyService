using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls
{      
    public sealed class ImageResourceModerator
    {
        #region static members (singleton & thread safety)
        
        private static ImageResourceModerator m_Instance = new ImageResourceModerator();
        private static object m_SyncRoot = new Object();

        private ImageResourceModerator()
        {
            //new ImageResourceModerator(); now is called from private static implicit constructor, which is supposed to be thread safe           
            //StartBackgroundProcessing(); //waiting for explicit call to start            
        }

        public static ImageResourceModerator Instance
        {
            get
            {             
                return m_Instance;
            }
        }

        #endregion       

        #region private members

        private byte m_NumberOfReferencesToKeepAlive = 5; //arbitrary number
        private FrameKey m_CurrentFrame = null;
        private FrameKey m_NextFrame = null;
        private FrameKey m_PreviousFrame = null;
        private FrameKey m_SignaledFrame = null;
        private FileResourceHandler m_FileResourceHandler = new FileResourceHandler();
        private Thread m_BackgroundProcessingThread;
        private AutoResetEvent m_ProcessFramesWaitHandle = new AutoResetEvent(false);        
        private bool m_Started = false;
        private CancellationTokenSource m_BackgroundProcessCancellationTokenSource = new CancellationTokenSource();

        #endregion

        #region public methods
              
        public BitmapSource GetBitmapSource(string filename, int frameIndex, string instanceID = "")
        {
 			if (frameIndex < 0)
			{
				return null;
			}

           BitmapSource bitmapSource = null;

            while (true)
            {
                if (Monitor.TryEnter(m_SyncRoot, 200))
                {
                    try
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        m_CurrentFrame = new FrameKey()
                        {
                            FileName = filename,
                            FrameIndex = frameIndex,
                            InstanceID = instanceID
                        };
                        //Console.WriteLine("Created current frame object " + m_CurrentFrame.ToString());
                        bitmapSource = m_FileResourceHandler.GetBitmapSource(m_CurrentFrame, cancellationTokenSource);
                        m_FileResourceHandler.GetRenderTargetBitmap(m_CurrentFrame, cancellationTokenSource); //prepare mask bitmap also

                        if (frameIndex > 0)
                        {
                            m_PreviousFrame = new FrameKey()
                            {
                                FileName = filename,
                                FrameIndex = frameIndex - 1,
                                InstanceID = instanceID
                            };
                            //Console.WriteLine("Created previous frame object " + m_PreviousFrame.ToString());
                        }
                        else
                        {
                            m_PreviousFrame = null;
                            //Console.WriteLine("Set previous frame object to null ");
                        }

                        if (frameIndex + 1 < m_FileResourceHandler.GetFramsCount(filename))
                        {
                            m_NextFrame = new FrameKey()
                            {
                                FileName = filename,
                                FrameIndex = frameIndex + 1,
                                InstanceID = instanceID
                            };
                            //Console.WriteLine("Created next frame object " + m_NextFrame.ToString());
                        }
                        else
                        {
                            m_NextFrame = null;
                            //Console.WriteLine("Set next frame object to null ");
                        }
                        m_ProcessFramesWaitHandle.Set(); //aaa
                    }
                    finally
                    {
                        Monitor.Exit(m_SyncRoot);
                        m_BackgroundProcessCancellationTokenSource = new CancellationTokenSource();                       
                    }
                    break;
                }
                else
                {
                    m_BackgroundProcessCancellationTokenSource.Cancel(); //cancel the operation that the background process used                    
                    Thread.Sleep(50);
                    //m_BackgroundProcessingThread.Abort();
                    //m_BackgroundProcessingThread = new Thread(new ThreadStart(ProcessPriorities));
                    //m_BackgroundProcessingThread.Priority = ThreadPriority.Lowest;
                    //m_BackgroundProcessingThread.Start();
                }
            }          

            return bitmapSource;
        }

        /// <summary>
        /// Will signal to moderator that next frames should be processes at idle time, even though there is no creation requirement for the current frame bitmap
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="frameIndex"></param>
        /// <param name="instanceID"></param>
        public void SignalToProcessFrames(string filename, int frameIndex, string instanceID = "")
        {
            //SignalToProcessFramesInternal(filename, frameIndex, instanceID);            
            if (m_SignaledFrame == null || m_SignaledFrame.FrameIndex != frameIndex)
            {
                m_SignaledFrame = new FrameKey() { FileName = filename, FrameIndex = frameIndex, InstanceID = instanceID };
                m_ProcessFramesWaitHandle.Set();
            }            
        }

        /// <summary>
        /// will be called immedialtely after same frame's bitmap source, should always have it ready in parallel with frame's bitmap source.
        /// no need for mutex here, because we are assuming there will be no case where object is not prepared.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="frameIndex"></param>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public RenderTargetBitmap GetRenderTargetBitmap(string fileName, int frameIndex, string instanceID = "")
        {
            RenderTargetBitmap renderTargetBitmap = null;
            //Console.WriteLine("GetRenderTargetBitmap external {0},{1}", filename, frameIndex.ToString());
            FrameKey frameKey = new FrameKey()
            {
                FileName = fileName,
                FrameIndex = frameIndex,
                InstanceID = instanceID
            };

            renderTargetBitmap = m_FileResourceHandler.GetRenderTargetBitmap(frameKey, new CancellationTokenSource());

            return renderTargetBitmap;
        }      

        public void StartBackgroundProcessing()
        {
            if (!m_Started)
            {
                m_BackgroundProcessingThread = new Thread(new ThreadStart(ProcessPriorities));
                m_BackgroundProcessingThread.Name = "Background Processing Thread";
                m_BackgroundProcessingThread.Priority = ThreadPriority.Lowest;
                m_BackgroundProcessingThread.IsBackground = true;
                m_BackgroundProcessingThread.Start();
                m_Started = true;
                //m_IsolatedStorageTimer = new Timer(new TimerCallback(TimerWake), null, 500, 0);                
            }
            else
            {
                if(Monitor.TryEnter (m_SyncRoot, 50)) //not critical to set next/previous frames to null
                {
                    try
                    {
                        m_NextFrame = null;
                        m_PreviousFrame = null;
                        m_CurrentFrame = null;
                        //m_FileResourceHandler.RecycleIsolatedStorage(); 
                    }
                    finally 
                    {                        
                        Monitor.Exit(m_SyncRoot);
                    }
                }
            }
            m_FileResourceHandler.StartIsolatedFileCopy();
        }

        /// <summary>
        /// Executes thread safe enqueue. Actuall copy of file to isolated storage will start when signaled.
        /// </summary>
        /// <param name="filename"></param>
        public void AddFileToIsolatedStorage(string filename)
        {
            m_FileResourceHandler.AddFileToIsolatedStorageQueue(filename);
        }

        public void ClearIsolatedStorage()
        {
            m_FileResourceHandler.ClearIsolatedStorage();
        }

        public void RecycleIsolatedStorage()
        {
            m_FileResourceHandler.RecycleIsolatedStorage();
        }        

        public void MarkCurrentFileSetAsRecycleable()
        {
            m_FileResourceHandler.MarkCurrenstFileSetAsRecycleable();
        }

        #endregion
        
        #region private methods        

        private void ProcessPriorities()
        {            
            while (true)
            {                
                if(Monitor.TryEnter(m_SyncRoot, 200))
                {
                    try
                    {
                        if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                        {
                            //Monitor.Exit(m_SyncRoot);
                            //m_ProcessFramesWaitHandle.WaitOne();
                            continue;
                        }

                        //Console.WriteLine("ProcessPriorities background entered");
                        if (m_FileResourceHandler != null && m_CurrentFrame != null)// && m_NextFrame != null && m_PreviousFrame != null)
                        {

                            if (m_SignaledFrame != null && m_SignaledFrame.FrameIndex != m_CurrentFrame.FrameIndex)
                            {
                                ChangeFramesAccordingToSignal();
                            }
                            else if (m_SignaledFrame != null && m_SignaledFrame.FrameIndex == m_CurrentFrame.FrameIndex)
                            {
                                m_SignaledFrame = null;
                            }

                            if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                            {
                                //Monitor.Exit(m_SyncRoot);
                                //m_ProcessFramesWaitHandle.WaitOne();
                                continue;
                            }

                            var ticks = DateTime.Now.Ticks;

                            if (m_NextFrame != null)
                            {
                                m_FileResourceHandler.GetBitmapSource(m_NextFrame, m_BackgroundProcessCancellationTokenSource);     //will load the bitmapsource and decoder into the dictionaries
                                if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                                {
                                    //Monitor.Exit(m_SyncRoot);
                                    //m_ProcessFramesWaitHandle.WaitOne();
                                    continue;
                                }
                                m_FileResourceHandler.GetRenderTargetBitmap(m_NextFrame, m_BackgroundProcessCancellationTokenSource);
                                if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                                {
                                    //Monitor.Exit(m_SyncRoot);
                                    //m_ProcessFramesWaitHandle.WaitOne();
                                    continue;
                                }
                                m_FileResourceHandler.SetScore(m_NextFrame, ticks - 1);
                            }

                            if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                            {
                                //Monitor.Exit(m_SyncRoot);
                                //m_ProcessFramesWaitHandle.WaitOne();
                                continue;
                            }

                            if (m_PreviousFrame != null)
                            {
                                m_FileResourceHandler.GetBitmapSource(m_PreviousFrame, m_BackgroundProcessCancellationTokenSource); //will load the bitmapsource and decoder into the dictionaries
                                if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                                {
                                    //Monitor.Exit(m_SyncRoot);
                                    //m_ProcessFramesWaitHandle.WaitOne();
                                    continue;
                                }
                                m_FileResourceHandler.GetRenderTargetBitmap(m_PreviousFrame, m_BackgroundProcessCancellationTokenSource);
                                if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                                {
                                    //Monitor.Exit(m_SyncRoot);
                                    //m_ProcessFramesWaitHandle.WaitOne();
                                    continue;
                                }
                                m_FileResourceHandler.SetScore(m_PreviousFrame, ticks - 2);
                            }

                            if (m_BackgroundProcessCancellationTokenSource.IsCancellationRequested)
                            {
                                //Monitor.Exit(m_SyncRoot);
                                //m_ProcessFramesWaitHandle.WaitOne();
                                continue;
                            }

                            m_FileResourceHandler.SetScore(m_CurrentFrame, ticks);
                            m_FileResourceHandler.RemoveLowScoreFrames(m_NumberOfReferencesToKeepAlive);
                        }
                        //Console.WriteLine("ProcessPriorities background leaving");
                        //TiffBitmapContainer.Mutex.ReleaseMutex();
                    }
                    finally 
                    {
                        Monitor.Exit(m_SyncRoot);
                    }
                }                
                //Console.WriteLine("m_WaitHandle.WaitOne() from ProcessPriorities. now sleeping.");
                m_ProcessFramesWaitHandle.WaitOne();//will be release by main thread when new bitmap is requested
            }
        }       

        private void ChangeFramesAccordingToSignal()
        {
            string filename = m_SignaledFrame.FileName;
            int frameIndex = m_SignaledFrame.FrameIndex;
            string instanceID = m_SignaledFrame.InstanceID;

            m_CurrentFrame = m_SignaledFrame;
            m_SignaledFrame = null;
                         
            //TiffBitmapDecoder tiffBitmapDecoder = m_FileResourceHandler.GetTiffBitmapDecoder(m_CurrentFrame.FileName);
            //int lastFrameIndex = 0;
            //tiffBitmapDecoder.Dispatcher.Invoke(new Action(() =>
            //{
            //    lastFrameIndex = tiffBitmapDecoder.Frames.Count - 1;

            //}));
            int lastFrameIndex = m_FileResourceHandler.GetFrameCount(filename) - 1;

            if (frameIndex > 0)
            {
                m_PreviousFrame = new FrameKey()
                {
                    FileName = filename,
                    FrameIndex = frameIndex - 1,
                    InstanceID = instanceID
                };
                //Console.WriteLine("Created previous frame object " + m_PreviousFrame.ToString());
            }
            else
            {
                m_PreviousFrame = null;
                //Console.WriteLine("Set previous frame object to null ");
            }

            if (frameIndex < lastFrameIndex)
            {
                m_NextFrame = new FrameKey()
                {
                    FileName = filename,
                    FrameIndex = frameIndex + 1,
                    InstanceID = instanceID
                };
                //Console.WriteLine("Created next frame object " + m_NextFrame.ToString());
            }
            else
            {
                m_NextFrame = null;
                //Console.WriteLine("Set next frame object to null ");
            }            
        }

        #endregion                     
    }

    public class FileResourceHandler
    {
        #region private members

        private ConcurrentDictionary<string, WeakReference> m_FrameBitmapSources = new ConcurrentDictionary<string, WeakReference>();
        private ConcurrentDictionary<string, WeakReference> m_FrameRenderTargetBitmaps = new ConcurrentDictionary<string, WeakReference>();
        private ConcurrentDictionary<string, WeakReference> m_TiffBitmapDecoders = new ConcurrentDictionary<string, WeakReference>();
        private ConcurrentDictionary<string, BitmapSource> m_FrameBitmapSourcesToForceKeep = new ConcurrentDictionary<string, BitmapSource>();
        private ConcurrentDictionary<string, RenderTargetBitmap> m_FrameRenderTargetBitmapsToForceKeep = new ConcurrentDictionary<string, RenderTargetBitmap>();
        private ConcurrentDictionary<string, int> m_TiffBitmapDecodersFrameCount = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, long> m_FrameScores = new ConcurrentDictionary<string, long>();
        private ConcurrentDictionary<string, List<string>> m_RenderTargetBitmapInstances = new ConcurrentDictionary<string, List<string>>();
        //private IsolatedStorageContainer m_IsolatedStorageContainer = new IsolatedStorageContainer();        

        #endregion               

        #region properties
        
        public IDictionary<string, long> FrameScores
        {
            get { return m_FrameScores; }
        }

        #endregion

        #region public methods

        public void StartIsolatedFileCopy()
        {
            //m_IsolatedStorageContainer.StartTimer();
        }

        public BitmapSource GetBitmapSource(FrameKey frameKey, CancellationTokenSource cancellationTokenSource)
        {
            //Console.WriteLine("GetBitmapSource internal START " + frameKey.ToString());
            BitmapSource bitmapSource = null;

            WeakReference weakReference = null;
            if (m_FrameBitmapSources.TryGetValue(frameKey.ToString(), out weakReference) == true)
            {
                bitmapSource = weakReference.Target as BitmapSource;
                //Console.WriteLine("GetBitmapSource internal, already contains bitmap source frame key (but perhaps weak reference is dead) " + frameKey.ToString());
                //Console.WriteLine(weakReference.IsAlive ? "hashed bitmap source is Alive" : "bitmap source Died");
            }

            if (bitmapSource == null) //means either weak reference was not found, or was found but target bitmapsource died
            {
                bitmapSource = GetTiffBitmapSource(frameKey.FileName, frameKey.FrameIndex, cancellationTokenSource);

                if (weakReference == null) //if earlier attempt to find key failed
                {
                    //if (m_FrameBitmapSources.TryGetValue(frameKey.ToString(), out weakReference) == false)
                    //{
                    m_FrameBitmapSources.TryAdd(frameKey.ToString(), weakReference = new WeakReference(bitmapSource));
                    //}
                }

                weakReference.Target = bitmapSource;
            }

            m_FrameBitmapSourcesToForceKeep.AddOrUpdate(frameKey.ToString(), bitmapSource, (k, x) => x == null ? bitmapSource : x);

            SetScore(frameKey, long.MaxValue);
            //Console.WriteLine("GetBitmapSource internal END " + frameKey.ToString());
            return bitmapSource;
        }

        public RenderTargetBitmap GetRenderTargetBitmap(FrameKey frameKey, CancellationTokenSource cancellationTokenSource)
        {
            //Console.WriteLine("GetRenderTargetBitmap internal START " + frameKey.ToString());
            RenderTargetBitmap renderTargetBitmap = null;

            WeakReference weakReference = null;
            if (m_FrameRenderTargetBitmaps.TryGetValue(frameKey.ToStringExt(), out weakReference) == true)
            {
                renderTargetBitmap = weakReference.Target as RenderTargetBitmap;
                //Console.WriteLine("GetBitmapSource internal, already contains bitmap source frame key (but perhaps weak reference is dead) " + frameKey.ToString());
                //Console.WriteLine(weakReference.IsAlive ? "hashed bitmap source is Alive" : "bitmap source Died");
            }

            if (renderTargetBitmap == null) //means either weak reference was not found, or was found but target bitmapsource died
            {
                renderTargetBitmap = GetNewRenderTargetBitmap(frameKey, cancellationTokenSource);

                if (weakReference == null) //if earlier attempt to find key failed
                {
                    //if (m_FrameBitmapSources.TryGetValue(frameKey.ToString(), out weakReference) == false)
                    //{                   
                    m_FrameRenderTargetBitmaps.TryAdd(frameKey.ToStringExt(), weakReference = new WeakReference(renderTargetBitmap));

                    m_RenderTargetBitmapInstances
                        .AddOrUpdate( 
                            frameKey.ToString(),
                            new List<string>() {frameKey.ToStringExt()}, 
                            (k,x) => 
                                { 
                                    x.Add(frameKey.ToStringExt()); 
                                    return x;
                                } );
                    //}
                }

                weakReference.Target = renderTargetBitmap;
            }

            m_FrameRenderTargetBitmapsToForceKeep
                .AddOrUpdate(frameKey.ToStringExt(), renderTargetBitmap, (k,x) => x == null ? renderTargetBitmap : x );

            //SetScore(frameKey, long.MaxValue); //no need to set score here, it is set when retrieving the bitmap source
            //Console.WriteLine("GetRenderTargetBitmap internal END " + frameKey.ToString());
            return renderTargetBitmap;
        }


        object m_lock = new object();
        public BitmapSource GetTiffBitmapSource(string filename, int pageIndex, CancellationTokenSource cancellationTokenSource)
        {
            BitmapSource result = null;
            lock (m_lock)
            {
                TiffBitmapDecoder tiffBitmapDecoder = null;
                while (tiffBitmapDecoder == null && !cancellationTokenSource.IsCancellationRequested)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        //using (MemoryStream fileMem = new MemoryStream(File.ReadAllBytes(filename)))
                        {
                            tiffBitmapDecoder = new TiffBitmapDecoder(new Uri(filename), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            m_TiffBitmapDecodersFrameCount[filename] = tiffBitmapDecoder.Frames.Count;
                            result = tiffBitmapDecoder.Frames[pageIndex];
                        }
                    }), TimeSpan.FromMilliseconds(500), null);
                }

                // Use GC.Collect to release the TiffBitmapDecoder memory - (.NET "Bug") 
                GC.Collect();

                return result;
            }
        }

        public RenderTargetBitmap GetNewRenderTargetBitmap(FrameKey frameKey, CancellationTokenSource cancellationTokenSource)
        {
            RenderTargetBitmap renderTargetBitmap = null;

            while (renderTargetBitmap == null && !cancellationTokenSource.IsCancellationRequested)
            {
                //tiffBitmapDecoder.Dispatcher.Invoke(new Action(() =>
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var bitmapSource = GetTiffBitmapSource(frameKey.FileName, frameKey.FrameIndex, cancellationTokenSource);
                    renderTargetBitmap = new System.Windows.Media.Imaging.RenderTargetBitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, 96, 96, System.Windows.Media.PixelFormats.Default);
                }), TimeSpan.FromMilliseconds(500), null);

            }
            //Console.WriteLine("renderTargetBitmap created " + frameKey.ToString());            
            return renderTargetBitmap;
        }

        public void SetScore(FrameKey frameKey, long ticks)
        {
            m_FrameScores.AddOrUpdate(frameKey.ToString(), ticks, (k, x) => ticks);
        }

        public void RemoveLowScoreFrames(int keepTop)
        {
            if (m_FrameBitmapSourcesToForceKeep.Count > keepTop)
            {
                List<string> frameKeyStringsToRemove = new List<string>();

                var frameKeysToKeep = GetTop(m_FrameScores, keepTop);
                foreach (var kp in m_FrameScores)
                {
                    if (!frameKeysToKeep.Contains<string>(kp.Key))
                    {
                        frameKeyStringsToRemove.Add(kp.Key);
                    }
                }

                //Bitmap Sources
                foreach (string frameKeyString in frameKeyStringsToRemove)
                {
                    if (m_FrameBitmapSourcesToForceKeep.ContainsKey(frameKeyString))
                    {
                        BitmapSource bitmap;
                        m_FrameBitmapSourcesToForceKeep.TryRemove(frameKeyString, out bitmap);
                        //Console.WriteLine("keep removed frame " + frameKeyString);
                    }
                }

                //RenderTargetBitmaps
                foreach (string frameKeyString in frameKeyStringsToRemove)
                {
                    List<string> instanceNames = null;
                    if (m_RenderTargetBitmapInstances.TryGetValue(frameKeyString, out instanceNames) == true)
                    {
                        foreach (string frameKeyStringExt in m_RenderTargetBitmapInstances[frameKeyString])
                        {
                            if (m_FrameRenderTargetBitmapsToForceKeep.ContainsKey(frameKeyStringExt))
                            {
                                RenderTargetBitmap tmp;
                                m_FrameRenderTargetBitmapsToForceKeep.TryRemove(frameKeyStringExt, out tmp);
                                //Console.WriteLine("keep render target bitmap removed frame " + frameKeyString);                            
                            }
                        }
                    }
                }

            }
        }

        public void AddFileToIsolatedStorageQueue(string filename)
        {
           // m_IsolatedStorageContainer.AddFileToStoragePending(filename);
        }       

        //public bool ToIsolatedStorageQueueHasFilesToAdd
        //{
        //    get
        //    {
        //        return m_IsolatedStorageContainer.HasQueuedFilesToAdd;
        //    }
        //}

        public void ClearIsolatedStorage()
        {
            //m_IsolatedStorageContainer.Clear();
        }

        public void RecycleIsolatedStorage()
        {
            //m_IsolatedStorageContainer.Recycle();
        }

        public int GetFrameCount(string filename)
        {
            return m_TiffBitmapDecodersFrameCount[filename];
        }


        public void MarkCurrenstFileSetAsRecycleable()
        {
            //m_IsolatedStorageContainer.MarkCurrenstFileSetAsRecycleable();
        }        

        #endregion       

        #region private methods

        private void Clear()
        {
            m_FrameBitmapSources = new ConcurrentDictionary<string, WeakReference>();
            m_FrameRenderTargetBitmaps = new ConcurrentDictionary<string, WeakReference>();
            m_TiffBitmapDecoders = new ConcurrentDictionary<string, WeakReference>();
            m_FrameBitmapSourcesToForceKeep = new ConcurrentDictionary<string, BitmapSource>();
            m_FrameRenderTargetBitmapsToForceKeep = new ConcurrentDictionary<string, RenderTargetBitmap>();
            m_TiffBitmapDecodersFrameCount = new ConcurrentDictionary<string, int>();
            m_FrameScores = new ConcurrentDictionary<string, long>();
            m_RenderTargetBitmapInstances = new ConcurrentDictionary<string, List<string>>();
        }
        //TODO: change this to sorted list or something faster.
        private IEnumerable<string> GetTop(IDictionary<string, long> dict, int takeTop)
        {
            var data = from key in dict.Keys
                       orderby dict[key] descending
                       select key;

            var t = data.Take<string>(takeTop);
            return t;
        }

        #endregion

        internal int GetFramsCount(string filename)
        {
            return m_TiffBitmapDecodersFrameCount[filename];
        }
    }

    public class FrameKey
    {        
        private const string format = "{0}|{1}";
        private const string formatExt = "{0}|{1}|{2}";
        public string FileName{get; set;}
        public int FrameIndex { get; set; }
        public string InstanceID { get; set; }
        
        public override string ToString()
        {
            return string.Format(format, FileName, FrameIndex.ToString("0"));
        }

        public string ToStringExt()
        {
            return string.Format(formatExt, FileName, FrameIndex.ToString("0"),InstanceID);
        }  
    }     
     
}
