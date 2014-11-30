using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.DataModel
{
    internal class ImportOverwriteInfo : IImportOverwriteInfo
    {
        private bool m_bOverwrite = false;
        private string m_sNewObjectName = String.Empty;

        public ImportOverwriteInfo(
            bool bOverwrite,
            string sNewObjectName)
        {
            m_bOverwrite = bOverwrite;
            m_sNewObjectName = sNewObjectName;
        }

        public ImportOverwriteInfo()
        {
        }

        public bool Overwrite
        {
            get { return m_bOverwrite; }
            set { m_bOverwrite = value; }
        }

        public string NewObjectName
        {
            get { return m_sNewObjectName; }
            set { m_sNewObjectName = value; }
        }
    }
}
