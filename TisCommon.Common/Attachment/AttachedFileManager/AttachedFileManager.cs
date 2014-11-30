using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TiS.Core.TisCommon.Storage;
using System.Collections.ObjectModel;
using System.Collections;
using TiS.Core.TisCommon.FilePath;

namespace TiS.Core.TisCommon.Attachment.File
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(false)]
    public class TisAttachedFileManager : ITisAttachedFileManager
    {
        private IStorageService m_SourceStorage;
        private IPathProvider m_LocalPathProvider;

        public TisAttachedFileManager(
            IStorageService sourceStorage,
            IPathProvider LocalPathProvider)
        {
            m_SourceStorage = sourceStorage;
            m_LocalPathProvider = LocalPathProvider;
        }

        #region Implementation of ITisAttachedFileManager

        public void RenameAttachment(string oldAttachedFileName, string newAttachedFileName)
        {
            m_SourceStorage.RenameStorage(oldAttachedFileName, newAttachedFileName);
        }

        public void CopyAttachment(string sourceAttachedFileName, string targetAttachedFileName, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction)
        {
            // Copy attachment
            StaticStorageServices.Copy(
                m_SourceStorage,
                m_SourceStorage,
                sourceAttachedFileName,
                targetAttachedFileName);
        }

        public void RemoveAllAttachments()
        {
            // Delete Storages (including sub-dirs)
            StaticStorageServices.DeleteStorage(m_SourceStorage, GetSourcePath(), "*.*", true);
        }

        public void RemoveAttachmentsOfTypes(string[] attachmentTypes)
        {
            foreach (string attachmentType in attachmentTypes)
            {
                RemoveAttachmentsOfType(attachmentType);
            }
        }

        public void RemoveAttachmentsOfType(string attachmentTypes)
        {
            // Delete BLOBs (including sub-dirs)
            StaticStorageServices.DeleteStorage(m_SourceStorage, GetSourcePath(), "*." + attachmentTypes, true);
        }

        public void RemoveAttachments(string[] attachedFileNames, bool isIgnoreErrors)
        {
            try
            {
                // !!! Continue to next BLOB on error
                m_SourceStorage.DeleteStorage(attachedFileNames);
            }
            catch (Exception)
            {
                if (!isIgnoreErrors)
                {
                    throw;
                }
            }
        }

        public void RemoveAttachment(string attachedFileName)
        {
            RemoveAttachments(new string[] { attachedFileName }, false);
        }

        public void SaveBLOBAsAttachment(MemoryStream binaryBuffer, string attachedFileName, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction)
        {
            // Save byte array
            SaveByteArrayAsAttachment(binaryBuffer.GetBuffer(), attachedFileName, enAttachmentExistsAction);
        }

        public void SaveByteArrayAsAttachment(byte[] buffer, string attachedFileName, TIS_ATTACHMENT_EXISTS_ACTION attachmentExistsAction)
        {
            m_SourceStorage.WriteStorage(attachedFileName, buffer);
        }

        public void SaveStringAsAttachment(string stringBuffer, string attachedFileName, TIS_ATTACHMENT_EXISTS_ACTION attachmentExistsAction)
        {
            SaveByteArrayAsAttachment(StringToBytes(stringBuffer), attachedFileName, attachmentExistsAction);
        }

        public void SaveAttachmentsEx(string filePath, string[] AttachedFileNames, TIS_ATTACHMENT_EXISTS_ACTION attachmentExistsAction)
        {
            foreach (string attachedFileName in AttachedFileNames)
            {
                SaveAttachment(System.IO.Path.Combine(filePath, attachedFileName), attachmentExistsAction);
            }
        }

        public void SaveAttachments(string[] attachedFilesFullNames, TIS_ATTACHMENT_EXISTS_ACTION attachmentExistsAction)
        {
            foreach (string attachedFileName in attachedFilesFullNames)
            {
                SaveAttachment(attachedFileName, attachmentExistsAction);
            }
        }

        public void SaveAttachment(string attachment, TIS_ATTACHMENT_EXISTS_ACTION attachmentExistsAction)
        {
            string localFileName;

            if (Path.IsPathRooted(attachment))
            {
                localFileName = attachment;
                attachment = AttachmentsUtil.GetAttachmentName(attachment);
            }
            else
            {
                localFileName = GetLocalFileName(attachment);
            }

            m_SourceStorage.WriteStorage(attachment, FileUtil.ReadAllBytes(localFileName));
        }

        public void SaveAttachment(string sourceAttachmentFile, string destinationAttachmentFile, TIS_ATTACHMENT_EXISTS_ACTION attachmentExistsAction)
        {
            string localFileName;

            if (Path.IsPathRooted(sourceAttachmentFile))
            {
                localFileName = sourceAttachmentFile;
            }
            else
            {
                localFileName = GetLocalFileName(sourceAttachmentFile);
            }

            m_SourceStorage.WriteStorage(destinationAttachmentFile, FileUtil.ReadAllBytes(localFileName));
        }

        #endregion

        #region Implementation of ITisAttachedFileManagerRead

        public ReadOnlyCollection<string> QueryAttachedTypes()
        {
            List<string> attachmentTypes = new List<string>();

            attachmentTypes.AddRange(StaticStorageServices.GetUsedExtenstions(m_SourceStorage, GetSourcePath(), true));

            ReadOnlyCollection<string> result = new ReadOnlyCollection<string>(attachmentTypes);
            return result;
        }

        public IList ExploreAttachments(string attachmentType)
        {
            return new List<string>(InternalQueryAttachmentsOfType(attachmentType));
        }

        public ReadOnlyCollection<string> QueryAttachments(string attachmentType)
        {
            return new ReadOnlyCollection<string>(InternalQueryAttachmentsOfType(attachmentType));
        }

        public IList ExploreAllAttachments()
        {
            return new List<string>(InternalQueryAllAttachments());
        }

        public ReadOnlyCollection<string> QueryAllAttachments()
        {
            return new ReadOnlyCollection<string>(InternalQueryAllAttachments());
        }

        public bool QueryAttachment(string attachedFileName)
        {
            string localFileName;

            if (Path.IsPathRooted(attachedFileName))
            {
                localFileName = attachedFileName;
                attachedFileName = AttachmentsUtil.GetAttachmentName(attachedFileName);
            }
            else
            {
                localFileName = GetLocalFileName(attachedFileName);
            }

            return System.IO.File.Exists(localFileName) || m_SourceStorage.IsStorageExist(attachedFileName); 
        }

        public MemoryStream GetAttachmentAsBLOB(string attachedFileName)
        {
            // Get attachment data as byte array
            byte[] data = GetAttachmentAsByteArray(attachedFileName);
            return new MemoryStream(data);
        }

        public byte[] GetAttachmentAsByteArray(string attachedFileName)
        {
            return m_SourceStorage.ReadStorage(attachedFileName);
        }

        public string GetAttachmentAsString(string attachedFileName)
        {
            return BytesToString(GetAttachmentAsByteArray(attachedFileName));
        }

        public int GetAttachmentsOfType(
            string attachmentType,
            string attachedFilePath)
        {
            // Query for attachments of specified type
            string[] attachments = InternalQueryAttachmentsOfType(attachmentType);

            // Redirect to GetAttachments
            return GetAttachmentsEx(attachedFilePath, attachments);
        }

        public int GetAttachmentsEx(
            string filePath,
            string[] attachedFileNames)
        {
            // Create full local path for each attachment
            // using the specified base path & attachment names
            string[] LocalFileNames = GetFullFileNames(
                filePath,
                attachedFileNames);

            // Redirect to GetAttachments
            return GetAttachments(LocalFileNames);
        }

        public int GetAttachments(string[] attachedFilesFullNames)
        {
            int successCount = 0;

            foreach (string fileName in attachedFilesFullNames)
            {
                try
                {
                    GetAttachment(fileName);

                    successCount++;
                }
                catch (Exception ex)
                {
                    Log.WriteError(
                        "GetAttachment([{0}]) failed", fileName);

                    Log.WriteException(ex);
                }

            }

            return successCount;
        }

        public void GetAttachment(string localFileName)
        {
            GetAttachment(localFileName, null);
        }

        public void GetAttachment(
            string localFileName,
            string remoteFileName = null)
        {
            try
            {
                if (!StringUtil.IsStringInitialized(remoteFileName))
                {
                    remoteFileName = Path.GetFileName(localFileName);
                }

                // Read
                byte[] data = m_SourceStorage.ReadStorage(remoteFileName);

                // Write
                FileUtil.CreateFileFromByteArray(localFileName, data);
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to read attachment [{0}] directly, loading from stream. Details : [{1}]", remoteFileName, exc.Message);

                Directory.CreateDirectory(Path.GetDirectoryName(localFileName));

                using (BufferedStream destinationStream = new BufferedStream(System.IO.File.Open(localFileName, FileMode.Create)))
                {
                    using (MemoryStream sourceStream = new MemoryStream(m_SourceStorage.ReadStorage(remoteFileName)))
                    {
                        // Write data
                        sourceStream.CopyTo(destinationStream);
                    }

                    destinationStream.Flush();
                }
            }
        }

        public DateTime GetAttachmentDateTime(string attachedFileName)
        {
            if (Path.IsPathRooted(attachedFileName))
            {
                attachedFileName = System.IO.Path.GetFileName(attachedFileName);
            }

            StorageInfo storageInfo = m_SourceStorage.GetStorageInfo(attachedFileName, StorageInfoFlags.LastModified);

            return storageInfo.LastModified;
        }

        public ITisAttachmentInfo[] QueryAttachmentsInfo(string attachmentNameFilter, string attachmentTypeFilter)
        {
            string sNameFilterToUse = "*";
            string typeFilterToUse = "*";

            if (StringUtil.IsStringInitialized(attachmentNameFilter))
            {
                sNameFilterToUse = attachmentNameFilter;
            }

            if (StringUtil.IsStringInitialized(attachmentTypeFilter))
            {
                typeFilterToUse = attachmentTypeFilter;
            }

            // Create filter
            string filter = String.Format(
                "{0}.{1}",
                sNameFilterToUse,
                typeFilterToUse);

            // Perform query

            List<StorageInfo> storagesInfo = new List<StorageInfo>(
                m_SourceStorage.QueryStorageInfo(String.Empty, filter, StorageInfoFlags.Size | StorageInfoFlags.LastModified));

            string[] subDirs = m_SourceStorage.QuerySubDirs(String.Empty, true);

            foreach (var subdir in subDirs)
            {
                StorageInfo[] subDirsStoragesInfo = 
                    m_SourceStorage.QueryStorageInfo(subdir, filter, StorageInfoFlags.Size | StorageInfoFlags.LastModified);

                foreach(StorageInfo storageInfo in subDirsStoragesInfo)
                {
                    storagesInfo.Add(new StorageInfo(
                        Path.Combine(subdir, storageInfo.Name),
                        storageInfo.SizeBytes,
                        storageInfo.CRC32,
                        storageInfo.LastModified,
                        storageInfo.DataFlags));
                }
            }

            ITisAttachmentInfo[] attachmentInfoArray = new ITisAttachmentInfo[storagesInfo.Count];

            // Repack IBLOBInfo to ITisAttachmentInfo

            for (int i = 0; i < storagesInfo.Count; i++)
            {
                IStorageInfo storageInfo = storagesInfo[i];

                attachmentInfoArray[i] = new AttachmentInfo(
                    storageInfo.Name,
                    AttachmentsUtil.GetAttachmentType(storageInfo.Name),
                    storageInfo.SizeBytes,
                    storageInfo.LastModified,
                    storageInfo.CRC32);
            }

            return attachmentInfoArray;
        }

        #endregion

        public IStorageService Storage
        {
            get
            {
                return m_SourceStorage;
            }
        }

        //
        //	Private
        //

        private string GetSourcePath()
        {
            return String.Empty;
        }

        private string[] InternalQueryAttachments(string filter)
        {
            return m_SourceStorage.QueryStorage(GetSourcePath(), filter);
        }

        private string[] InternalQueryAllAttachments()
        {
            return InternalQueryAttachments("*.*");
        }

        private string[] InternalQueryAttachmentsOfType(string typeOfAttachment)
        {
            return InternalQueryAttachments("*." + typeOfAttachment);
        }

        private string[] GetFullFileNames(string path, string[] fileNames)
        {
            List<string> fullFileNames = new List<string>();

            foreach (string fileName in fileNames)
            {
                fullFileNames.Add(System.IO.Path.Combine(path, fileName));
            }

            return fullFileNames.ToArray();
        }

        private byte[] StringToBytes(string str)
        {
            return StringEncoding.GetBytes(str);
        }

        private string BytesToString(byte[] bytes)
        {
            return StringEncoding.GetString(bytes);
        }

        private System.Text.Encoding StringEncoding
        {
            get
            {
                return new System.Text.UnicodeEncoding();
            }
        }

        private string GetLocalFileName(string attachment)
        {
            string localPath =
                m_LocalPathProvider.GetPath(attachment);

            // Return full path
            return System.IO.Path.Combine(localPath, attachment);
        }
    }
}
