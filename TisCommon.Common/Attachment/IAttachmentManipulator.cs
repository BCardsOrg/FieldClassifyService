using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TiS.Core.TisCommon.Attachment
{
    public interface IAttachmentManipulator
    {
		/// <summary>
		/// Gets the supported attachment types. (Ex: "TIF")
		/// </summary>
        List<string> SupportedAttachmentTypes { get; }

		/// <summary>
		/// Copies the attachment pages to the end of destination attachment file
		/// </summary>
		/// <param name="sourceAttachmentFile">The source attachment file.</param>
		/// <param name="destinantionAttachmentFile">The destination attachment file.</param>
		/// <param name="pageNumbers">The page indexes. (zero base)</param>
        void CopyAttachmentPages(string sourceAttachmentFile, string destinantionAttachmentFile, IEnumerable<int> pageNumbers);

		/// <summary>
		/// Removes the attachment pages.
		/// </summary>
		/// <param name="attachmentFile">The attachment file.</param>
		/// <param name="pageNumbers">The page indexes. (zero base)</param>
        void RemoveAttachmentPages(string attachmentFile, IEnumerable<int> pageNumbers);
   }
}
