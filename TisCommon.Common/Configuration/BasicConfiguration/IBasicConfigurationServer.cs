using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.Configuration
{
    /// <summary>
    /// Interface that includes eFLOW basic configuration data and actions.
    /// </summary>
    [Guid("E041D895-CAA5-42F7-A517-294274F8390F")]   
    public interface IBasicConfigurationServer : IBasicConfigurationDataServer, IConfigurationActions
    {
    }
}
