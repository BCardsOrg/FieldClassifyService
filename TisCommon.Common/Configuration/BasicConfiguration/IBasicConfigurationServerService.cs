using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace TiS.Core.TisCommon.Configuration
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBasicConfigurationServerService" in both code and config file together.
    [ServiceContract(Namespace = "http://www.topimagesystems.com/Core/TisCommon/Configuration/BasicConfigurationServerService")]
    public interface IBasicConfigurationServerService
    {
        [OperationContract]
        BasicConfigurationDataServer GetConfiguration();

        [OperationContract]
        void SaveConfiguration(BasicConfigurationDataServer configuration);  
    }
}
