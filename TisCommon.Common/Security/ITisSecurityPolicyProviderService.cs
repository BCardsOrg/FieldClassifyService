using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace TiS.Core.TisCommon.Security
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = "http://www.topimagesystems.com/Core/Domain/Security/TisSecurityPolicyProviderService")]
    public interface ITisSecurityPolicyProviderService
    {

        TisSecurityPolicy TisSecurityPolicy { [OperationContract] get; } 
    }
}
