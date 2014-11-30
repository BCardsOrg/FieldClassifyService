using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Attachment
{
    public class DefaultAttachmentManipulator : IAttachmentManipulator
    {
        #region IAttachmentManipulator Members

        public List<string> SupportedAttachmentTypes
        {
            get 
            {
                return new List<string>(new string[] { "TIF", "REG" });
            }
        }

        public void CopyAttachmentPages(
            string sourceAttachmentFile, 
            string destinantionAttachmentFile, 
            IEnumerable<int> pageNumbers)
        {
            ImageHelper.CopyImageFile(
                sourceAttachmentFile,
                destinantionAttachmentFile,
                pageNumbers);
        }

        public void RemoveAttachmentPages(
            string attachmentFile, 
            IEnumerable<int> pageNumbers)
        {
            ImageHelper.RemovePagesFromImageFile(
                attachmentFile,
                pageNumbers);
        }

        #endregion
    }
}
