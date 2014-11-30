using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TiS.Core.Common
{
	public static class WorkflowBinaryReader
	{
        public static Binary ReadWorkflow(string workflowFile)
		{
			using (var fs = File.OpenRead(workflowFile))
			{
				return ReadWorkflow(fs);
			}
		}

        public static Binary ReadWorkflow(Stream source)
		{
			using (var br = new BinaryReader(source, Encoding.ASCII))
			{
				var version = br.ReadInt32();
				if (version != 0x30000) throw new InvalidDataException("Unknown file format.");

				var numberOfNodes = br.ReadInt32();
				var numberOfRoutes = br.ReadInt32();

                var nodes = new List<Binary.Node>();
				for (int i = 0; i < numberOfNodes; i++)
				{
					var node = ReadNode(br);
					nodes.Add(node);
				}

                var routes = new List<Binary.Route>();
				for (int i = 0; i < numberOfRoutes; i++)
				{
					var route = ReadRoute(br);
					route.SourceId = nodes[route.SourceIndex].Id;
					route.TargetId = nodes[route.TargetIndex].Id;
					routes.Add(route);
				}

                return new Binary { Nodes = nodes, Routes = routes };
			}
		}
		
		private static string ReadString(BinaryReader br, int maxLength = 0)
		{
			var length = br.ReadByte();
			if (length == 0 || (maxLength > 0 && length > maxLength))
			{
				return string.Empty;
			}

			var text = new string(br.ReadChars(length));
			return text;
		}

        private static Binary.Node ReadNode(BinaryReader br)
		{
            var node = new Binary.Node();
			node.Id = br.ReadInt32();

			// Very broad sanity check
			if ((node.Id < 0) || (node.Id > 500)) throw new InvalidDataException("Failed to read the workflow file.");

			node.X = br.ReadInt32();
			node.Y = br.ReadInt32();
			node.Width = br.ReadInt32() - node.X;
			node.Height = br.ReadInt32() - node.Y;
			
			// Skip ~garbage~
			br.ReadBytes(4*6);

			node.Color = br.ReadInt32();

			// Skip ~garbage~
			br.ReadBytes(4 * 2);

			while (string.IsNullOrEmpty(node.Name))
				node.Name = ReadString(br, 42);

			while (string.IsNullOrEmpty(node.Properties) || !node.Properties.Contains(";"))
				node.Properties = ReadString(br);
			
			// Trailing \0
			br.ReadByte();

			// Skip ~garbage~
			br.ReadBytes(0x1D);

			// Image
			var imageHeader = br.ReadInt16();
			if (imageHeader == 0x4D42)
			{
				var imageLength = br.ReadInt32();
				var imageData = br.ReadBytes(imageLength - 2 - 4);
				node.Image = new byte[0].
					Concat(BitConverter.GetBytes(imageHeader)).
					Concat(BitConverter.GetBytes(imageLength)).
					Concat(imageData).ToArray();

				// Skip ~garbage~
				br.ReadBytes(0x2A);
			}
			else
			{
				// Skip ~garbage~
				br.ReadBytes(0xF);
			}
			
			return node;
		}

        private static Binary.Route ReadRoute(BinaryReader br)
		{
			var route = new Binary.Route();
			route.Id = br.ReadInt32();

			// Very broad sanity check
			if ((route.Id < 0) || (route.Id > 500)) throw new InvalidDataException("Failed to read the workflow file.");

			route.SourceIndex = br.ReadInt32() - 1;
			route.TargetIndex = br.ReadInt32() - 1;

			// Skip ~garbage~
			br.ReadBytes(0x1F);

			while (string.IsNullOrEmpty(route.Properties) || !route.Properties.Contains(";"))
				route.Properties = ReadString(br);
			// Trailing \0
			br.ReadByte();

			// Skip ~garbage~
			br.ReadBytes(0x2D);

			route.Points = new List<Tuple<int, int>>();
			var numberOfPoints = br.ReadInt32();
			for (int i = 0; i < numberOfPoints; i++)
			{
				route.Points.Add(Tuple.Create(br.ReadInt32(), br.ReadInt32()));
			}

			return route;
		}
	}
}
