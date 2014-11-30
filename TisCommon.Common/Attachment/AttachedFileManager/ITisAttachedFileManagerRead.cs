using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Collections;

namespace TiS.Core.TisCommon.Attachment.File
{
    [Guid("3AC48920-55D4-4A66-A4AA-A357B072EC46")]
    public interface ITisAttachedFileManagerReadCOM
    {
        void GetAttachment(String sAttachedFileFullName);
        bool QueryAttachment(String sAttachedFileName);
        IList ExploreAllAttachments();
        IList ExploreAttachments(String sTypeOfAttachment);
    }

    [ComVisible(false)]
    public interface ITisAttachedFileManagerRead : ITisAttachedFileManagerReadCOM
    {
        DateTime GetAttachmentDateTime(String sAttachedFileName);
        int GetAttachments(String[] AttachedFilesFullNames);

        int GetAttachmentsEx(String sFilePath, String[] AttachedFileNames);

        int GetAttachmentsOfType(String sTypeOfAttachment, String sAttachedFilePath);

        string GetAttachmentAsString(String sAttachedFileName);

        byte[] GetAttachmentAsByteArray(String sAttachedFileName);

        MemoryStream GetAttachmentAsBLOB(String sAttachedFileName);

        ReadOnlyCollection<string> QueryAllAttachments();

        ReadOnlyCollection<string> QueryAttachments(String sTypeOfAttachment);

        ReadOnlyCollection<string> QueryAttachedTypes();

        ITisAttachmentInfo[] QueryAttachmentsInfo(string AttachmentNameFilter, string AttachmentTypeFilter);

        void GetAttachment(
            string localFileName,
            string remoteFileName = null);
    }
}
