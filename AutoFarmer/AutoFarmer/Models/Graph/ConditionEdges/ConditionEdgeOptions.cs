using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.Conditions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AutoFarmer.Models.Graph.IOptions;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public class ConditionEdgeOptions : ConditionEdgeBase, IOptions
	{
		public Dictionary<string, List<object>>? TemplateVariables { get; set; }

		public Dictionary<string, string>? Nodes { get; set; }

		public ConditionOptions? Condition { get; set; }

		public dynamic PreferredOrder { get; set; } = 1;

		public static List<ConditionEdge> FromJsonFile(string path)
		{
			return FromJsonFileWrapper(() =>
			{
				var edgeOptions = JsonConvert.DeserializeObject<ConditionEdgeOptions>(File.ReadAllText(path));

				if (edgeOptions.Nodes is null)
				{
					var arr = Path.GetFileNameWithoutExtension(path).Split('-');

					edgeOptions.Nodes = new Dictionary<string, string>() { { arr[0], arr[1] } };
				}

				List<ConditionEdge> result = new List<ConditionEdge>();

				foreach (var nodes in edgeOptions.Nodes)
				{
					if (edgeOptions.TemplateVariables is { } && IsContainVariable(edgeOptions.TemplateVariables.Keys.ToList(), nodes.Key, nodes.Value))
					{
						for (int i = 0; i < edgeOptions.TemplateVariables.First().Value.Count; i++)
						{
							var startNodeName = ReplaceVariables(nodes.Key, edgeOptions.TemplateVariables, i);
							var endNodeName = ReplaceVariables(nodes.Value, edgeOptions.TemplateVariables, i);

							var condition = edgeOptions.Condition?.Clone();

							if (condition is { })
							{
								condition.ReplaceVariablesInCondition(edgeOptions.TemplateVariables, i);
							}

							int prefferedOrder;

							if (edgeOptions.PreferredOrder is string s)
							{
								if (IsContainVariable(edgeOptions.TemplateVariables.Keys.ToList(), s))
								{
									var preferredOrderName = ReplaceVariables(s, edgeOptions.TemplateVariables, i);

									prefferedOrder = int.Parse(preferredOrderName);
								}
								else
								{
									prefferedOrder = int.Parse(s);
								}
							}
							else
							{
								prefferedOrder = edgeOptions.PreferredOrder;
							}

							result.Add(edgeOptions.ToConditionEdge(startNodeName, endNodeName, prefferedOrder, condition, edgeOptions.Flags));
						}
					}
					else
					{
						result.Add(edgeOptions.ToConditionEdge(nodes.Key, nodes.Value, edgeOptions.PreferredOrder, edgeOptions.Condition, edgeOptions.Flags));
					}
				}

				return result;
			});
		}

		private ConditionEdge ToConditionEdge(string startNodeName, string endNodeName, int preferredOrder, ConditionOptions? conditionOptions, ConditionEdgeFlags? flags)
		{
			return new ConditionEdge(startNodeName, endNodeName, Order, preferredOrder, conditionOptions?.ToCondition(), MaxCrossing, ConsiderationProbability, flags);
		}
	}
}
