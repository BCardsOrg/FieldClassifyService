using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;

namespace TiS.Core.TisCommon.Attachment
{
    [Guid("AF56091A-10CB-4C61-AA94-292416C9C267")]
    public interface ISupportsAttachmentsCOM
    {
        IList GetLocalAttachments(); 
    }

    [ComVisible(false)]
    public interface ISupportsAttachments : ISupportsAttachmentsCOM
    {
        string GetAttachmentFileName(String sAttachmentType);

        string GetAttachmentFileNameWithoutPath(String sAttachmentType);

        string GetAttachmentNameByFileName(string attachmentFile);

        IList<string> LocalAttachments { get; }
    }
}
