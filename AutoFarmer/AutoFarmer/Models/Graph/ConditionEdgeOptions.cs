using AutoFarmer.Models.ImageMatching;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.GraphNamespace
{
	public class ConditionEdgeOptions : Options
	{
		public Dictionary<string, List<object>> TemplateParameters { get; set; }

		public string Name { get; set; }

		public Dictionary<string, string> Nodes { get; set; }

		public Conditions Conditions { get; set; }

		public int MaxCrossing { get; set; } = 1;

		public int CurrentCrossing { get; set; }

		public int Order { get; set; } = 1;

		public static List<ConditionEdge> FromJsonFile(string path)
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
				if (edgeOptions.TemplateParameters != null && IsContainsTemplate(edgeOptions.TemplateParameters.Keys.ToList(), tuple.Key, tuple.Value))
				{
					for (int i = 0; i < edgeOptions.TemplateParameters.First().Value.Count; i++)
					{
						var startNodeName = ReplaceTemplates(tuple.Key, edgeOptions.TemplateParameters, i);
						var endNodeName = ReplaceTemplates(tuple.Value, edgeOptions.TemplateParameters, i);

						var preCondition = edgeOptions.Conditions?.PreCondition?.Clone();
						var postCondition = edgeOptions.Conditions?.PostCondition?.Clone();

						if (preCondition != null && IsContainsTemplate(edgeOptions.TemplateParameters.Keys.ToList(), preCondition.TemplateName))
						{
							preCondition.TemplateName = ReplaceTemplates(preCondition.TemplateName, edgeOptions.TemplateParameters, i);
							preCondition.SearchRectangleName = ReplaceTemplates(preCondition.SearchRectangleName, edgeOptions.TemplateParameters, i);
						}

						if (postCondition != null && preCondition != postCondition && IsContainsTemplate(edgeOptions.TemplateParameters.Keys.ToList(), postCondition.TemplateName))
						{
							postCondition.TemplateName = ReplaceTemplates(postCondition.TemplateName, edgeOptions.TemplateParameters, i);
							postCondition.SearchRectangleName = ReplaceTemplates(postCondition.SearchRectangleName, edgeOptions.TemplateParameters, i);
						}

						result.Add(CreateConditionEdge(startNodeName, endNodeName, edgeOptions.Order, edgeOptions.MaxCrossing, new Conditions() { PreCondition = preCondition, PostCondition = postCondition }));
					}
				}
				else
				{
					result.Add(CreateConditionEdge(tuple.Key, tuple.Value, edgeOptions.Order, edgeOptions.MaxCrossing, edgeOptions.Conditions));
				}
			}

			return result;
		}

		private static ConditionEdge CreateConditionEdge(string startNodeName, string endNodeName, int order, int maxCrossing, Conditions conditions)
		{
			return new ConditionEdge()
			{
				Conditions = conditions,
				MaxCrossing = maxCrossing,
				Order = order,
				StartNodeName = startNodeName,
				EndNodeName = endNodeName,
				Name = $"{startNodeName}-{endNodeName}"
			};
		}
	}
}
