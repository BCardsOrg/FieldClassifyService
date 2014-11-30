using System;
namespace TiS.Core.TisCommon.Configuration
{
    public interface IBasicConfigurationProviderHelper
    {
        [Obsolete("Method is deprecated", true)]
        void SetDataPath(string configFilesPath);
        GlobalConfigurationService Storage { get; set; }
    }
}
