using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.Conditions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public class ConditionEdgeOptions : ConditionEdgeBase
	{
		public Dictionary<string, List<object>> TemplateVariables { get; set; }

		public Dictionary<string, string> Nodes { get; set; }

		public ConditionOptions Condition { get; set; }

		public static List<ConditionEdge> FromJsonFile(string path)
		{
			return Shared.FromJsonFileWrapper(() =>
			{
				var edgeOptions = JsonConvert.DeserializeObject<ConditionEdgeOptions>(File.ReadAllText(path));
				edgeOptions.Name = Path.GetFileNameWithoutExtension(path);

				if (edgeOptions.Nodes is null)
				{
					var arr = edgeOptions.Name.Split('-');

					edgeOptions.Nodes = new Dictionary<string, string>() { { arr[0], arr[1] } };
				}

				List<ConditionEdge> result = new List<ConditionEdge>();

				foreach (var tuple in edgeOptions.Nodes)
				{
					if (edgeOptions.TemplateVariables != null && Shared.IsContainVariable(edgeOptions.TemplateVariables.Keys.ToList(), tuple.Key, tuple.Value))
					{
						for (int i = 0; i < edgeOptions.TemplateVariables.First().Value.Count; i++)
						{
							var startNodeName = Shared.ReplaceVariables(tuple.Key, edgeOptions.TemplateVariables, i);
							var endNodeName = Shared.ReplaceVariables(tuple.Value, edgeOptions.TemplateVariables, i);

							var condition = edgeOptions.Condition?.Clone();

							if (condition != null && Shared.IsContainVariable(edgeOptions.TemplateVariables.Keys.ToList(), condition.TemplateName))
							{
								condition.ReplaceVariablesInCondition(edgeOptions.TemplateVariables, i);
							}

							result.Add(edgeOptions.ToConditionEdge(startNodeName, endNodeName, edgeOptions.Order, edgeOptions.MaxCrossing, edgeOptions.Condition));
						}
					}
					else
					{
						result.Add(edgeOptions.ToConditionEdge(tuple.Key, tuple.Value, edgeOptions.Order, edgeOptions.MaxCrossing, edgeOptions.Condition));
					}
				}

				return result;
			});
		}

		private ConditionEdge ToConditionEdge(string startNodeName, string endNodeName, int order, int maxCrossing, ConditionOptions conditionOptions)
		{
			return new ConditionEdge()
			{
				Condition = conditionOptions?.ToCondition(),
				MaxCrossing = maxCrossing,
				Order = order,
				StartNodeName = startNodeName,
				EndNodeName = endNodeName,
				Name = $"{startNodeName}-{endNodeName}"
			};
		}
	}
}
