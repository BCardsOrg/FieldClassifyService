using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Principal;
//using Microsoft.IdentityModel.Claims;

namespace TiS.Core.TisCommon.Services.Web
{
    #region TisServiceContextData

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public sealed class TisWebServiceContextData
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string InstanceName { get; set; }
        [DataMember]
        public string CreatorTypeName { get; set; }
        [DataMember]
        public string NodeName { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string StationName { get; set; }
		[DataMember]
		public string StationId { get; set; }
		[DataMember]
		public byte [] InstancePermission { get; set; }
		[DataMember]
		public string UserName { get; set; }

    }

    #endregion
}
