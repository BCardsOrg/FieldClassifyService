using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Configuration
{
    public class BasicConfigurationProviderHelper : IBasicConfigurationProviderHelper
    {
        private GlobalConfigurationService m_storage;

        public BasicConfigurationProviderHelper()
        {
            m_storage = new GlobalConfigurationService();

        }

        public GlobalConfigurationService Storage
        {
            get { return m_storage; }
            set { m_storage = value; }
        }

        [Obsolete("Method is deprecated", true)]
        public void SetDataPath(string configFilesPath)
        {
        }
    }
}
