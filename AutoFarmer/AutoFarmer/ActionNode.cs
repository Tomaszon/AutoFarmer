using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer
{
	public class ActionNode
	{
		public string Name { get; set; }

		public NodeActions Actions { get; set; }

		public static ActionNode FromJsonFile(string path)
		{
			var node = JsonConvert.DeserializeObject<ActionNode>(File.ReadAllText(path));
			node.Name = Path.GetFileNameWithoutExtension(path);

			return node;
		}
	}
}
