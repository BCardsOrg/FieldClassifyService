using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Attachment
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AttachmentsUtil
    {
        // Returns AttType (only extension, without dot)
        public static string GetAttachmentType(string attachment)
        {
            string sAttType = System.IO.Path.GetExtension(attachment);

            return sAttType.Replace(".", "").ToUpper();
        }

        // Returns Att.AttType (without path)
        public static string GetAttachmentName(string attachmentFile)
        {
            string sAttachmentWithoutPath =
                System.IO.Path.GetFileName(attachmentFile);

            return sAttachmentWithoutPath;
        }

        // Returns Att.AttType (without path)
        public static string GetAttachmentNameWithoutType(string attachmentFile)
        {
            return System.IO.Path.GetFileNameWithoutExtension(
                GetAttachmentName(attachmentFile));
        }

    }
}
