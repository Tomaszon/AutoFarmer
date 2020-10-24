using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AutoFarmer.Models.Graph.IOptions;

namespace AutoFarmer.Models.Graph.ActionNodes
{
	public class ActionNodeOptions : ActionNodeBase, IOptions
	{
		public Dictionary<string, List<object>>? TemplateVariables { get; set; }

		public string[]? Names { get; set; }

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
					if (nodeOptions.TemplateVariables is { } && IsContainVariable(nodeOptions.TemplateVariables.Keys.ToList(), name))
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
			return new ActionNode(name)
			{
				Actions = Actions,
				AdditionalDelayAfterLastAction = AdditionalDelayAfterLastAction,
				AdditionalDelayBetweenActions = AdditionalDelayBetweenActions,
				IsStartNode = IsStartNode,
				IsEndNode = IsEndNode
			};
		}
	}
}
