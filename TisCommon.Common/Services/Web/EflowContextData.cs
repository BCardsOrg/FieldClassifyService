using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Services.Web
{
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public sealed class EflowContextData
    {
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string StationId { get; set; }
        [DataMember]
        public string UserName { get; set; }
    }
}
