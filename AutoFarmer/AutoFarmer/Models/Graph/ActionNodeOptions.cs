using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer.Models.GraphNamespace
{
	public class ActionNodeOptions
	{
		public string[] Names { get; set; }

		public NodeActions Actions { get; set; }

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; set; }

		public bool IsVisited { get; set; }

		public static ActionNodeOptions FromJsonFile(string path)
		{
			var nodeOptions = JsonConvert.DeserializeObject<ActionNodeOptions>(File.ReadAllText(path));

			if (nodeOptions.Names is null)
			{
				nodeOptions.Names = new[] { Path.GetFileNameWithoutExtension(path) };
			}

			return nodeOptions;
		}
	}
}
