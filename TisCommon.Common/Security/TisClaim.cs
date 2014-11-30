using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Security
{
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
	public class TisClaim
	{
		[DataMember]
		public TisClaimResource Resource { get; set; }
		[DataMember]
		public TisClaimOperation Operation { get; set; }
		[DataMember]
		public string ExtraData { get; set; }
	}
}
