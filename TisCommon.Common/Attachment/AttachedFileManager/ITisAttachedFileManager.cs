using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Attachment.File
{
    public enum TIS_ATTACHMENT_EXISTS_ACTION
    {
        TIS_EXISTING_APPEND = 2,
        TIS_EXISTING_ERROR = 3,
        TIS_EXISTING_IGNORE = 1,
        TIS_EXISTING_OVERRIDE = 0
    }

    [Guid("19551f64-afd2-4141-9465-971c6f90497f")]
    public interface ITisAttachedFileManager : ITisAttachedFileManagerRead
    {
        void SaveAttachment(String sAttachedFileFullName, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction);

        void SaveAttachments(String[] AttachedFilesFullNames, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction);

        void SaveAttachmentsEx(String sFilePath, String[] AttachedFileNames, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction);

        void SaveStringAsAttachment(String sStringBuffer, String sAttachedFileName, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction);

        void SaveByteArrayAsAttachment(Byte[] Buffer, String sAttachedFileName, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction);

        void SaveBLOBAsAttachment(MemoryStream opBinaryBuffer, String sAttachedFileName, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction);

        void RemoveAttachment(String sAttachedFileName);

        void RemoveAttachments(String[] AttachedFileNames, Boolean bIgnoreErrors);

        void RemoveAttachmentsOfType(String sAttachmentType);

        void RemoveAttachmentsOfTypes(String[] AttachmentTypes);

        void RemoveAllAttachments();

        void CopyAttachment(String sSourceAttachedFileName, String sTargetAttachedFileName, TIS_ATTACHMENT_EXISTS_ACTION enAttachmentExistsAction);

        void RenameAttachment(String sOldAttachedFileName, String sNewAttachedFileName);
    }
}
