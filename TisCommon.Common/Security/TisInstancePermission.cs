using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Security
{
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
	public class TisInstancePermission
	{
		[DataMember]
		public List<TisClaim> TisClaims { get; private set; }

		public TisInstancePermission()
		{
			TisClaims = new List<TisClaim>();
		}
	}
}
