using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer
{
	public class ActionNode
	{
		public string Name { get; set; }

		public NodeActions Actions { get; set; }

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; internal set; }

		public bool IsVisited { get; set; }

		public void ResetState()
		{
			IsVisited = false;
		}

		public static ActionNode FromJsonFile(string path)
		{
			var node = JsonConvert.DeserializeObject<ActionNode>(File.ReadAllText(path));
			node.Name = Path.GetFileNameWithoutExtension(path);

			return node;
		}
	}
}
