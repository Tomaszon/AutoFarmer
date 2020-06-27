using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.Graph
{
	public class ActionNodeOptions : Options
	{
		public Dictionary<string, List<object>> TemplateParameters { get; set; }

		public string[] Names { get; set; }

		public NodeActions Actions { get; set; }

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; set; }

		public bool IsVisited { get; set; }

		public static List<ActionNode> FromJsonFile(string path)
		{
			var nodeOptions = JsonConvert.DeserializeObject<ActionNodeOptions>(File.ReadAllText(path));

			if (nodeOptions.Names is null)
			{
				nodeOptions.Names = new[] { Path.GetFileNameWithoutExtension(path) };
			}

			List<ActionNode> result = new List<ActionNode>();

			foreach (var name in nodeOptions.Names)
			{
				if (nodeOptions.TemplateParameters != null && IsContainsTemplate(nodeOptions.TemplateParameters.Keys.ToList(), name))
				{
					for (int j = 0; j < nodeOptions.TemplateParameters.First().Value.Count; j++)
					{
						var modifiedName = ReplaceTemplates(name, nodeOptions.TemplateParameters, j);

						result.Add(CreateActionNode(modifiedName, nodeOptions.Actions, nodeOptions.IsStartNode, nodeOptions.IsEndNode));
					}
				}
				else
				{
					result.Add(CreateActionNode(name, nodeOptions.Actions, nodeOptions.IsStartNode, nodeOptions.IsEndNode));
				}
			}

			return result;
		}

		private static ActionNode CreateActionNode(string name, NodeActions actions, bool isStartNode, bool isEndNode)
		{
			return new ActionNode()
			{
				Name = name,
				Actions = actions,
				IsStartNode = isStartNode,
				IsEndNode = isEndNode
			};
		}
	}
}
