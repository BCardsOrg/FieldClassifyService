using System.IO;
using TiS.Core.TisCommon.Attachment;

namespace TiS.Core.TisCommon.FilePath
{
    public class LocalPathProvider : IPathProvider
    {
        private LocalPathLocator m_pathLocator;
        //private bool m_isWebSession;
        //private string m_WebDataUserStorageFolder;

        public LocalPathProvider(
            LocalPathLocator pathLocator,
            string userName = "",
            bool isWebSession = false)
		{
			m_pathLocator = pathLocator;
        }

        public string SpecificPath { get; set; }

		public string GetPath(string sFileName)
		{
            if (StringUtil.IsStringInitialized(SpecificPath))
            {
                return SpecificPath;
            }
            else
            {
                string sExt = AttachmentsUtil.GetAttachmentType(sFileName);

                // If the file has no extension, error
                if (sExt.Length == 0)
                {
                    throw new TisException(
                        "The file name [{0}] has no extension, " +
                        "can't determine target path");
                }

                // Use PathLocator to determine target path
                return m_pathLocator[sExt];
            }
		}
    }
}
