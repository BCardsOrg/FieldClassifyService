using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Collections
{
	/// <summary>
	/// This class reduce the size of the Dictionary serialization process
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[Serializable]
	[CollectionDataContract
		(Namespace = "http://www.topimagesystems.com/Core/TisCommon/Collections",
		Name = "X",
		ItemName = "N",
		KeyName = "K",
		ValueName = "V")]
	public class TisWcfDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{ }
}
