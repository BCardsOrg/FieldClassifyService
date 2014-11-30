using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TiS.Core.Common
{
	public class Binary
	{
		public IList<Node> Nodes { get; set; }
		public IList<Route> Routes { get; set; }

		[DebuggerDisplay("Workflow Node {Id}: {Name}")]
		public class Node
		{
			public int Id { get; set; }
			public int X { get; set; }
			public int Y { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public int Color { get; set; }
			public string Name { get; set; }
			public string Properties { get; set; }
			public byte[] Image { get; set; }
		}

		[DebuggerDisplay("Workflow Route {Id}: {SourceId} -> {TargetId}")]
		public class Route
		{
			public int Id { get; set; }
			public int SourceId { get; set; }
			public int TargetId { get; set; }
			public int SourceIndex { get; set; }
			public int TargetIndex { get; set; }
			public string Properties { get; set; }
			public IList<Tuple<int, int>> Points { get; set; }
		}
	}
}
