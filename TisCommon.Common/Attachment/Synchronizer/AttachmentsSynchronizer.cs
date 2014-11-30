using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using TiS.Core.TisCommon.Storage;
using TiS.Core.TisCommon.Attachment.File;
using TiS.Core.TisCommon.FilePath;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TiS.Core.TisCommon.Attachment.Synchronizer
{
    [ComVisible(false)]
    public enum TisAttachmentsSynchronizationPolicy { None, Different, RemoteNew, LocalNew };

    [ComVisible(false)]
    public enum TisAttachmentsOperation { Download, Upload };

    [ComVisible(false)]
    public interface ITisAttachmentsSynchronizationContext
    {
        TisAttachmentsOperation AttachmentsOperation { get; }
        ITisAttachmentsFilter AttachmentsFilter { get; set; }
        TisAttachmentsSynchronizationPolicy AttachmentsSynchronizationPolicy { get; set; }
        bool IsEnabled { get; set; }
    }

    [ComVisible(false)]
    public delegate void AttachmentsSynchronizationDelegate(List<string> attachmentOperationCandidates, bool isSyncCall = true);

    #region AttachmentsSynchronizer

    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    // TO_DO: Change to internal and move to client....
    public class AttachmentsSynchronizer
    {
        protected ITisAttachedFileManager m_AttachedFileManager;

        private IPathProvider m_LocalPathProvider;

        private List<string> m_DownloadedAttachments = new List<string>();
        private List<string> m_UploadedAttachments = new List<string>();

        private string m_InputDir;

        protected AttachmentInfoMap m_RemoteAttachmentsInfo = new AttachmentInfoMap();

        private List<ITisAttachmentsSynchronizationContext> m_AttachmentsSynchronizationContexts = new List<ITisAttachmentsSynchronizationContext>();

        //
        //	Public
        //

        public AttachmentsSynchronizer(ITisAttachedFileManager attachedFileManager, IPathProvider localPathProvider, string inputDir)
        {
            m_LocalPathProvider = localPathProvider;
            m_AttachedFileManager = attachedFileManager;
            m_InputDir = inputDir;
        }

        public AttachmentsSynchronizer(
            ITisAttachedFileManager attachedFileManager,
            IPathProvider localPathProvider,
            string inputDir,
            List<ITisAttachmentsSynchronizationContext> attachmentsSynchronizationContexts)
            : this(attachedFileManager, localPathProvider, inputDir)
        {
            foreach (ITisAttachmentsSynchronizationContext attachmentsSynchronizationContext in attachmentsSynchronizationContexts)
            {
                AddAttachmentsSynchronizationContext(attachmentsSynchronizationContext);
            }
        }

        public event AttachmentOperationCompletedDelegate OnAttachmentOperationDone;

        // TO_DO: Change to internal and move to client....
        public virtual ITisAttachmentsSynchronizationContext AddAttachmentsSynchronizationContext(
            TisAttachmentsOperation attachmentsOperation,
            ITisAttachmentsFilter attachmentsFilter,
            TisAttachmentsSynchronizationPolicy attachmentsSynchronizationPolicy)
        {
            ITisAttachmentsSynchronizationContext attachmentsSynchronizationContext = new AttachmentsSynchronizationContext(
                attachmentsOperation,
                attachmentsFilter,
                attachmentsSynchronizationPolicy);

            AddAttachmentsSynchronizationContext(attachmentsSynchronizationContext);

            return attachmentsSynchronizationContext;
        }

        public virtual void AddAttachmentsSynchronizationContext(ITisAttachmentsSynchronizationContext attachmentsSynchronizationContext)
        {
            if (!m_AttachmentsSynchronizationContexts.Contains(attachmentsSynchronizationContext))
            {
                m_AttachmentsSynchronizationContexts.Add(attachmentsSynchronizationContext);
            }
        }

        public void RemoveAttachmentsSynchronizationContext(ITisAttachmentsSynchronizationContext attachmentsSynchronizationContext)
        {
            if (m_AttachmentsSynchronizationContexts.Contains(attachmentsSynchronizationContext))
            {
                m_AttachmentsSynchronizationContexts.Remove(attachmentsSynchronizationContext);
            }
        }

        public void EnableAttachmentsSynchronizationContext(bool enable)
        {
            foreach (ITisAttachmentsSynchronizationContext attachmentsSynchronizationContext in m_AttachmentsSynchronizationContexts)
            {
                attachmentsSynchronizationContext.IsEnabled = enable;
            }
        }

        public string InputDir
        {
            get { return m_InputDir; }
            set { m_InputDir = value; }
        }

        public List<string> DownloadedAttachments
        {
            get
            {
                return GetLocalFiles(m_DownloadedAttachments);
            }
        }

        public List<string> UploadedAttachments
        {
            get
            {
                return GetLocalFiles(m_UploadedAttachments);
            }
        }
        public IEnumerable<string> DownloadedAttachmentNames
        {
            get
            {
                return m_DownloadedAttachments;
            }
            set
            {
                m_DownloadedAttachments.Clear();

                m_DownloadedAttachments.AddRange(GetLocalFiles(value));
            }
        }

        public IEnumerable<string> UploadedAttachmentNames
        {
            get
            {
                return m_UploadedAttachments;
            }
            set
            {
                m_UploadedAttachments.Clear();

                m_UploadedAttachments.AddRange(GetLocalFiles(value));
            }
        }


        //
        //	Downloads attachments that match the m_oDownloadAttachmentsFilter,
        //	puts it in local locations according to oLocalPathProvider.
        //	All downloaded attachments are recorded and can be automatically 
        //  saved or deleted later.
        //
        public void Download(
            bool forceDownload = false,
            bool isSyncCall = true)
        {
            Download(null, forceDownload, isSyncCall);
        }

        public void Download(
            List<string> downloadCandidates,
            bool forceDownload = false,
            bool isSyncCall = true)
        {
            if (downloadCandidates == null)
            {
                downloadCandidates = GetDownloadCandidates(forceDownload);
            }

            DownloadAttachments(downloadCandidates, forceDownload, isSyncCall);
        }

        //
        //	Saves all attachments that were downloaded and also those that
        //	located in input directory (m_sInputDir). All saved attachments must 
        //  pass the m_oUploadAttachmentsFilter.
        //
        public void Upload(
            bool IsChunkedMode,
            bool forceUpload = false,
            bool isSyncCall = false)
        {
            Upload(null, forceUpload, isSyncCall, IsChunkedMode);
        }


        public void Upload(
            List<string> uploadCandidates,
            bool forceUpload = false,
            bool isSyncCall = false,
            bool IsChunkedMode = false)
        {
            if (uploadCandidates == null)
            {
                // Get the files that were downloaded or present in local dir
                uploadCandidates = GetUploadCandidates();
            }

            UploadAttachments(uploadCandidates, forceUpload, isSyncCall, IsChunkedMode);
        }


        //
        //	Deletes all attachments that were downloaded or uploaded
        //
        public void DeleteLocal()
        {
            // Delete uploaded attachments
            DeleteUploadedLocal();

            // Remove all current local attachments
            DeleteFiles(LocalAttachments);
        }

        private void DeleteDownloadedLocal()
        {
            lock (this)
            {
                // Remove downloaded

                DeleteFiles(GetLocalFiles(m_DownloadedAttachments));

                // Clear list of downloaded files
                m_DownloadedAttachments.Clear();
            }
        }

        private void DeleteUploadedLocal()
        {
            lock (this)
            {
                // Remove uploaded
                DeleteFiles(GetLocalFiles(m_UploadedAttachments));

                // Clear list of uploaded files
                m_UploadedAttachments.Clear();
            }
        }

        // Returns all downloaded attachments + all attachments that
        // can be uploaded
        public string[] LocalAttachments
        {
            get
            {
                lock (this)
                {
                    Hashtable oLocalAttachments = new Hashtable(500);

                    foreach (string sAtt in GetLocalFiles(m_DownloadedAttachments))
                    {
                        oLocalAttachments[sAtt] = sAtt;
                    }

                    List<string> UploadCandidates = GetUploadCandidates();

                    foreach (string sAtt in GetLocalFiles(UploadCandidates))
                    {
                        oLocalAttachments[sAtt] = sAtt;
                    }

                    ICollection oKeys = oLocalAttachments.Keys;

                    string[] Attachments = new string[oKeys.Count];
                    oKeys.CopyTo(Attachments, 0);

                    return Attachments;
                }
            }
        }

        public string GetLocalFileName(string attachmentFile)
        {
            string localPath = Path.GetDirectoryName(attachmentFile);

            if (m_LocalPathProvider is LocalPathLocator)
            {
                if (StringUtil.IsStringInitialized(localPath))
                {
                    return Path.Combine(((LocalPathLocator)m_LocalPathProvider).DefaultRoot, attachmentFile);
                }

                localPath = m_LocalPathProvider.GetPath(AttachmentsUtil.GetAttachmentType(attachmentFile));
            }
            else
            {
                localPath = m_LocalPathProvider.GetPath(attachmentFile);
            }

            // Return full path
            return Path.Combine(localPath, attachmentFile);
        }

        //
        //	Protected
        //

        protected ITisAttachedFileManager AttachedFileManager
        {
            get
            {
                return m_AttachedFileManager;
            }
            set
            {
                m_AttachedFileManager = value;
            }
        }

        protected virtual bool NeedUpload(
            string attachment,
            TisAttachmentsSynchronizationPolicy attachmentsComparePolicy)
        {
            // Don't need to save if local file not exists
            if (!System.IO.File.Exists(GetLocalFileName(attachment)))
            {
                return false;
            }

            // Return true if files are different
            return AreLocalAndSourceDifferent(
                attachment,
                attachmentsComparePolicy);
        }

        protected virtual bool NeedDownload(
            string attachment,
            TisAttachmentsSynchronizationPolicy attachmentsComparePolicy)
        {
            return AreLocalAndSourceDifferent(
                attachment,
                attachmentsComparePolicy
                );
        }

        public List<string> GetDownloadCandidates(bool remoteRefresh)
        {
            List<string> oAttachmentsToDownload = new List<string>();

            RefreshRemoteAttachmentsInfo(remoteRefresh);

            ITisAttachmentInfo[] AttachmentsInfo =
                m_RemoteAttachmentsInfo.AttachmentsInfo;

            foreach (ITisAttachmentInfo oAttachmentInfo in AttachmentsInfo)
            {
                string sAttachment = oAttachmentInfo.AttachmentName;

                // If passes filter
                if (IsPass(TisAttachmentsOperation.Download, sAttachment, new AttachmentOperationNeededDelegate(NeedDownload)))
                {
                    oAttachmentsToDownload.Add(sAttachment);
                }
                else
                {
                    Log.WriteDetailedDebug("Skipping attachment [{0}] ...", sAttachment);
                }
            }

            return oAttachmentsToDownload;
        }

        public List<string> GetUploadCandidates()
        {
            List<string> attachmentsToUpload = new List<string>();

            List<string> uploadCandidates = new List<string>(InputDirAttachments);

            RefreshRemoteAttachmentsInfo(false);

            ITisAttachmentInfo[] AttachmentsInfo =
                m_RemoteAttachmentsInfo.AttachmentsInfo;

            foreach (ITisAttachmentInfo oAttachmentInfo in AttachmentsInfo)
            {
                if (!uploadCandidates.Contains(oAttachmentInfo.AttachmentName))
                {
                    uploadCandidates.Add(oAttachmentInfo.AttachmentName);
                }
            }

			foreach (var attachment in uploadCandidates)
			{
				if (IsPass(TisAttachmentsOperation.Upload, attachment, new AttachmentOperationNeededDelegate(NeedUpload)))
				{
					attachmentsToUpload.Add(attachment);
				}
				else
				{
					Log.WriteDetailedDebug("Skipping attachment [{0}] ...", attachment);
				}
			}

            return attachmentsToUpload;
        }

        public virtual void RenameAttachments(
            Regex regEx,
            string newAttachmentFile)
        {
            string renamedAttachmentFile;

            foreach (string originalAttachmentFile in DownloadedAttachments)
            {
                renamedAttachmentFile = regEx.Replace(originalAttachmentFile, newAttachmentFile);

                RenameLocalAttachment(m_DownloadedAttachments, originalAttachmentFile, renamedAttachmentFile);
            }

            foreach (string originalAttachmentFile in UploadedAttachments)
            {
                renamedAttachmentFile = regEx.Replace(originalAttachmentFile, newAttachmentFile);

                RenameLocalAttachment(m_UploadedAttachments, originalAttachmentFile, renamedAttachmentFile);
            }
        }

        private void RenameLocalAttachment(
            List<string> localAttachments,
            string originalAttachmentFile,
            string newAttachmentFile)
        {
            if (!StringUtil.CompareIgnoreCase(originalAttachmentFile, newAttachmentFile))
            {
                originalAttachmentFile = Path.GetFileName(originalAttachmentFile);
                newAttachmentFile = Path.GetFileName(newAttachmentFile);

                localAttachments.Remove(originalAttachmentFile);
                localAttachments.Add(newAttachmentFile);
            }
        }


        //
        //	Private methods
        //

        private delegate bool AttachmentOperationNeededDelegate(string sAttachment, TisAttachmentsSynchronizationPolicy attachmentsComparePolicy);

        private bool IsPass(
            TisAttachmentsOperation attachmentsOperation,
            string attachment,
            AttachmentOperationNeededDelegate isAttachmentOperationNeeded)
        {
            foreach (ITisAttachmentsSynchronizationContext attachmentsSynchronizationContext in m_AttachmentsSynchronizationContexts)
            {
                if (attachmentsSynchronizationContext.IsEnabled &&
                    attachmentsOperation == attachmentsSynchronizationContext.AttachmentsOperation)
                {
                    if (attachmentsSynchronizationContext.AttachmentsFilter == null ||
                       attachmentsSynchronizationContext.AttachmentsFilter.IsPass(attachment))
                    {
                        if (isAttachmentOperationNeeded(attachment, attachmentsSynchronizationContext.AttachmentsSynchronizationPolicy))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void DeleteFiles(IEnumerable<string> FileNames)
        {
            foreach (string sFile in FileNames)
            {
                Log.WriteDetailedDebug("Deleting attachment file [{0}]", sFile);

                if (System.IO.File.Exists(sFile))
                {
                    System.IO.File.Delete(sFile);
                }
            }
        }

        private void DownloadAttachments(
            List<string> Attachments,
            bool forceUpload = false,
            bool isSyncCall = true)
        {
            AttachmentsOperation(
                Attachments,
                new AttachmentOperationDelegate(DownloadAttachment),
                forceUpload,
                isSyncCall);
        }

        private void UploadAttachments(
            List<string> Attachments,
            bool forceUpload = false,
            bool isSyncCall = true,
            bool IsChunkedMode = false)
        {
            AttachmentsOperation(
                Attachments,
                new AttachmentOperationDelegate(UploadAttachment),
                forceUpload,
                isSyncCall,
                IsChunkedMode);
        }

        private void AttachmentsOperation(
            List<string> Attachments,
            AttachmentOperationDelegate attachmentOperationDelegate,
            bool forceOperation = false,
            bool isSyncCall = true,
            bool IsChunkedMode = false)
        {
            isSyncCall = isSyncCall || Attachments.Count < 2;

            if (!isSyncCall)
            {
                Parallel.ForEach(Attachments, attachment => attachmentOperationDelegate(attachment, forceOperation, IsChunkedMode));
				AttachmentOperationDone(null);
            }
            else
            {
                Attachments.ForEach(attachment => attachmentOperationDelegate(attachment, forceOperation, IsChunkedMode));

                AttachmentOperationDone(null);
            }
        }

        private void AttachmentOperationDone(IAsyncResult ar)
        {
            if (OnAttachmentOperationDone != null)
            {
                OnAttachmentOperationDone(this);
            }
        }

        private delegate void AttachmentOperationDelegate(string sAttachment, bool forceOperation = false, bool IsChunkedMode = false);

        protected virtual void DownloadAttachment(
            string sAttachment,
            bool forceOperation = false, 
            bool IsChunkedMode = false)
        {
            // Obtain local file name
            string sLocalFileName =
                GetLocalFileName(sAttachment);

            // Download attachment in most optimal way

            if (IsSmallRemoteAttachment(sAttachment))
            {
                GetSmallAttachment(sLocalFileName, sAttachment);
            }
            else
            {
                GetLargeAttachment(sLocalFileName, sAttachment);
            }

            // Update local file LastWriteTime according to remote file
            // In order to allow comparison later

            ITisAttachmentInfo oAttInfo = m_RemoteAttachmentsInfo.CheckedGetAttachmentInfo(sAttachment);

            FileInfo oDownloadedFileInfo = new FileInfo(sLocalFileName);

            try
            {
                oDownloadedFileInfo.LastWriteTime = oAttInfo.LastModified;
            }
            // File maybe write by other process, so leave the LastWriteTime
            catch (System.IO.IOException)
            {
            }

            // Register attachment in downloaded attachments list
            RegisterDownloadedAttachment(sAttachment);
        }

        private void GetLargeAttachment(
            string localFileName,
            string remoteFileName)
        {
            // Get attachment
            m_AttachedFileManager.GetAttachment(localFileName, remoteFileName);
        }

        // Optimization for small attachments
        private void GetSmallAttachment(
            string localFileName, 
            string remoteFileName)
        {
            // Get attachment as byte array
            byte[] data = m_AttachedFileManager.GetAttachmentAsByteArray(remoteFileName);

            // Write to file
            FileUtil.CreateFileFromByteArray(localFileName, data);
        }

        private bool IsSmallRemoteAttachment(string sAttachment)
        {
            ITisAttachmentInfo oAttInfo =
                m_RemoteAttachmentsInfo.CheckedGetAttachmentInfo(sAttachment);

            if (IsSmallAttachment(oAttInfo.SizeBytes))
            {
                return true;
            }

            return false;
        }

        protected bool IsSmallAttachment(long lSizeBytes)
        {
            if (lSizeBytes <= FileStorageConfiguration.SmallBlobLimit)
            {
                return true;
            }

            return false;
        }

        protected virtual void UploadAttachment(
            string attachment,
            bool forceUpload = false, 
            bool IsChunkedMode = false)
        {
            string localFileName = GetLocalFileName(attachment);

            FileInfo oFileInfo = new FileInfo(localFileName);

            if (IsSmallAttachment(oFileInfo.Length))
            {
                byte[] Data = FileUtil.GetFileAsByteArray(localFileName);

                m_AttachedFileManager.SaveByteArrayAsAttachment(
                    Data,
                    attachment,
                    TIS_ATTACHMENT_EXISTS_ACTION.TIS_EXISTING_OVERRIDE);
            }
            else
            {
                // Get attachment
                m_AttachedFileManager.SaveAttachment(
					localFileName,
                    TIS_ATTACHMENT_EXISTS_ACTION.TIS_EXISTING_OVERRIDE);
            }

            // Register attachment in uploaded attachments list
            RegisterUploadedAttachment(attachment);

            UpdateRemoteAttachmentsCache(oFileInfo);
        }

        private bool AreLocalAndSourceDifferent(
            string attachemnt,
            TisAttachmentsSynchronizationPolicy attachmentsComparePolicy)
        {
            if (attachmentsComparePolicy == TisAttachmentsSynchronizationPolicy.None)
            {
                return true;
            }

            string localFileName = GetLocalFileName(attachemnt);

            FileInfo oFileInfo = new FileInfo(localFileName);

            if (!oFileInfo.Exists)
            {
                // No local file = different
                return true;
            }

            ITisAttachmentInfo oRemoteAttInfo =
                m_RemoteAttachmentsInfo.GetAttachmentInfo(attachemnt);

            if (oRemoteAttInfo == null)
            {
                // No remote attachment = different
                return true;
            }

            if (attachmentsComparePolicy == TisAttachmentsSynchronizationPolicy.LocalNew)
            {
                return oFileInfo.LastWriteTime > oRemoteAttInfo.LastModified;
            }

            if (attachmentsComparePolicy == TisAttachmentsSynchronizationPolicy.RemoteNew)
            {
                return oFileInfo.LastWriteTime < oRemoteAttInfo.LastModified;
            }

            if (oFileInfo.LastWriteTime != oRemoteAttInfo.LastModified ||
                oFileInfo.Length != oRemoteAttInfo.SizeBytes)
            {
                // Different modify time -> different
                return true;
            }

            // The same
            return false;
        }

        protected void RegisterDownloadedAttachment(string sAttachment)
        {
            lock (m_DownloadedAttachments)
            {
                if (!m_DownloadedAttachments.Contains(sAttachment))
                {
                    m_DownloadedAttachments.Add(sAttachment);
                }
            }
        }

        protected void RegisterUploadedAttachment(string attachment)
        {
            lock (m_UploadedAttachments)
            {
                if (!m_UploadedAttachments.Contains(attachment))
                {
                    m_UploadedAttachments.Add(attachment);
                }
            }
        }

        private List<string> QueryInputDirAttachments()
        {
            List<string> attachments = new List<string>();

            if (!StringUtil.IsStringInitialized(m_InputDir) ||
                !Directory.Exists(m_InputDir))
            {
                return attachments;
            }

            string[] files = Directory.GetFiles(m_InputDir, "*.*");

            foreach (var file in files)
            {
                string attachment;

                if (m_LocalPathProvider is LocalPathLocator)
                {
                    attachment = file.Remove(0, ((LocalPathLocator)m_LocalPathProvider).DefaultRoot.Length);
                }
                else
                {
                    attachment = Path.GetFileName(file);
                }

                attachments.Add(attachment);
            }

            return attachments;
        }

        private ICollection<string> DownloadedAndInputDirAttachments
        {
            get
            {
                lock (this)
                {
                    string[] downloadedAttachmentsArray = m_DownloadedAttachments.ToArray();
                    string[] inputDirAttachments = QueryInputDirAttachments().ToArray();

                    ICollection<string> union = StringUtil.GetUnion(downloadedAttachmentsArray, inputDirAttachments);
                    return union;
                }
            }
        }

        private ICollection<string> InputDirAttachments
        {
            get
            {
                lock (this)
                {
                    return QueryInputDirAttachments();
                }
            }
        }

        protected virtual void RefreshRemoteAttachmentsInfo(bool remoteRefresh)
        {
            ITisAttachmentInfo[] InfoArray = m_AttachedFileManager.QueryAttachmentsInfo(
                String.Empty,
                String.Empty);

            UpdateRemoteAttachmentsInfo(InfoArray);
        }
        protected virtual void UpdateRemoteAttachmentsCache(FileInfo attachmentFileInfo)
        {
        }

        protected void UpdateRemoteAttachmentsInfo(ITisAttachmentInfo[] InfoArray)
        {
            lock (this)
            {
                m_RemoteAttachmentsInfo.Clear();

                m_RemoteAttachmentsInfo.Add(InfoArray);
            }
        }

        private List<string> GetLocalFiles(IEnumerable<string> attachments)
        {
            List<string> localFiles = new List<string>();

            foreach (var attachment in attachments)
            {
                localFiles.Add(GetLocalFileName(attachment));
            }

            return localFiles;
        }
    }

    #endregion


    #region AttachmentInfoMap

    public class AttachmentInfoMap
    {
        // AttachmentName -> ITisAttachmentInfo
        private Hashtable m_oNameToInfoMap;

        public AttachmentInfoMap()
        {
            //m_oNameToInfoMap = new Hashtable(
            //    new CaseInsensitiveHashCodeProvider(), 
            //    new CaseInsensitiveComparer());
            // Changed after moving from .NET 1.1
            // TODO : check whether we should use InvariantCulture or CurrentCulture
            m_oNameToInfoMap = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Add(ITisAttachmentInfo[] InfoArray)
        {
            foreach (ITisAttachmentInfo oInfo in InfoArray)
            {
                m_oNameToInfoMap.Add(
                    oInfo.AttachmentName,
                    oInfo);
            }
        }

        public void Clear()
        {
            m_oNameToInfoMap.Clear();
        }

        public ITisAttachmentInfo GetAttachmentInfo(
            string sAttachmentName)
        {
            return (ITisAttachmentInfo)m_oNameToInfoMap[sAttachmentName];
        }

        public ITisAttachmentInfo CheckedGetAttachmentInfo(
            string sAttachmentName)
        {
            ITisAttachmentInfo oInfo = GetAttachmentInfo(sAttachmentName);

            if (oInfo == null)
            {
                throw new TisException("No {0} entry found for sAttachmentName={1}",
                    typeof(ITisAttachmentInfo),
                    sAttachmentName);
            }

            return oInfo;
        }

        public void UpdateAttachmentInfo(string attachmentName, ITisAttachmentInfo attachmentInfo)
        {
            if (m_oNameToInfoMap.Contains(attachmentName))
            {
                m_oNameToInfoMap[attachmentName] = attachmentInfo;
            }
        }

        public ITisAttachmentInfo[] AttachmentsInfo
        {
            get
            {
                ITisAttachmentInfo[] RetArray = new ITisAttachmentInfo[m_oNameToInfoMap.Count];

                m_oNameToInfoMap.Values.CopyTo(RetArray, 0);

                return RetArray;
            }
        }
    }

    #endregion

    public delegate void AttachmentOperationCompletedDelegate(object sender);

    #region AttachmentsSynchronizationContext

    public class AttachmentsSynchronizationContext : ITisAttachmentsSynchronizationContext
    {
        private TisAttachmentsOperation m_attachmentsOperation;
        private ITisAttachmentsFilter m_attachmentsFilter;
        private TisAttachmentsSynchronizationPolicy m_attachmentsSynchronizationPolicy;
        private bool m_isEnabled = true;

        public AttachmentsSynchronizationContext()
        {
        }

        public AttachmentsSynchronizationContext(
            TisAttachmentsOperation attachmentsOperation,
            ITisAttachmentsFilter attachmentsFilter,
            TisAttachmentsSynchronizationPolicy attachmentsSynchronizationPolicy)
        {
            m_attachmentsOperation = attachmentsOperation;
            m_attachmentsFilter = attachmentsFilter;
            m_attachmentsSynchronizationPolicy = attachmentsSynchronizationPolicy;
        }

        #region ITisAttachmentsSynchronizationContext Members

        public TisAttachmentsOperation AttachmentsOperation
        {
            get
            {
                return m_attachmentsOperation;
            }
        }

        public ITisAttachmentsFilter AttachmentsFilter
        {
            get
            {
                return m_attachmentsFilter;
            }
            set
            {
                m_attachmentsFilter = value;
            }
        }

        public TisAttachmentsSynchronizationPolicy AttachmentsSynchronizationPolicy
        {
            get
            {
                return m_attachmentsSynchronizationPolicy;
            }
            set
            {
                m_attachmentsSynchronizationPolicy = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return m_isEnabled;
            }
            set
            {
                m_isEnabled = value;
            }
        }

        #endregion
    }

    #endregion
}
