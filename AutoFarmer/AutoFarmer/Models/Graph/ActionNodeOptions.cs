using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.Graph
{
	public class ActionNodeOptions : Options
	{
		public Dictionary<string, List<object>> TemplateVariables { get; set; }

		public string[] Names { get; set; }

		public string[] ActionNames { get; set; }

		public int AdditionalDelayBetweenActions { get; set; }

		public int AdditionalDelayAfterLastAction { get; set; }

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; set; }

		public bool IsVisited { get; set; }

		public static List<ActionNode> FromJsonFile(string path)
		{
			return FromJsonFileWrapper(() =>
			{
				var nodeOptions = JsonConvert.DeserializeObject<ActionNodeOptions>(File.ReadAllText(path));

				if (nodeOptions.Names is null)
				{
					nodeOptions.Names = new[] { Path.GetFileNameWithoutExtension(path) };
				}

				List<ActionNode> result = new List<ActionNode>();

				foreach (var name in nodeOptions.Names)
				{
					if (nodeOptions.TemplateVariables != null && IsContainVariable(nodeOptions.TemplateVariables.Keys.ToList(), name))
					{
						for (int j = 0; j < nodeOptions.TemplateVariables.First().Value.Count; j++)
						{
							var modifiedName = ReplaceVariables(name, nodeOptions.TemplateVariables, j);

							result.Add(nodeOptions.ToActionNode(modifiedName));
						}
					}
					else
					{
						result.Add(nodeOptions.ToActionNode(name));
					}
				}

				return result;
			});
		}

		private ActionNode ToActionNode(string name)
		{
			return new ActionNode()
			{
				Name = name,
				ActionNames = ActionNames,
				AdditionalDelayAfterLastAction = AdditionalDelayAfterLastAction,
				AdditionalDelayBetweenActions = AdditionalDelayBetweenActions,
				IsStartNode = IsStartNode,
				IsEndNode = IsEndNode
			};
		}
	}
}
