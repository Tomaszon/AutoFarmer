using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer
{
	public class Graph
	{
		public string[] StartNodeNames { get; set; }

		public Dictionary<string, ActionNode> ActionNodes { get; set; } = new Dictionary<string, ActionNode>();

		public Dictionary<string, ConditionEdge> ConditionEdges { get; set; } = new Dictionary<string, ConditionEdge>();

		public static Graph FromConfig(Config config)
		{
			var graph = JsonConvert.DeserializeObject<Graph>(File.ReadAllText(config.GraphConfigPath));

			foreach (var file in Directory.GetFiles(config.ActionNodesDirectory))
			{
				graph.ActionNodes.Add(Path.GetFileNameWithoutExtension(file), ActionNode.FromJsonFile(file));
			}
			foreach (var file in Directory.GetFiles(config.ConditionEdgesDirectory))
			{
				graph.ConditionEdges.Add(Path.GetFileNameWithoutExtension(file), ConditionEdge.FromJsonFile(file));
			}

			return graph;
		}

		public ActionNode GetNextNode(ConditionEdge conditionEdge)
		{
			return ActionNodes[conditionEdge.EndNodeName];
		}

		public ConditionEdge[] GetNextConditionEdges(ActionNode actionNode)
		{
			var res = ConditionEdges.Select(c => c.Value).Where(c => c.StartNodeName == actionNode.Name);

			return res.Count() == 0 ? null : res.ToArray();
		}
	}
}
