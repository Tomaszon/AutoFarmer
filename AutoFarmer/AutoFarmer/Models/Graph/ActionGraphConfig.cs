using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AutoFarmer.Models.Graph
{
	public class ActionGraphConfig
	{
		public ActionGraphFlagsConfig AddFlags { get; set; } = new ActionGraphFlagsConfig();

		public ActionGraphFlagsConfig RemoveFlags { get; set; } = new ActionGraphFlagsConfig();

		public Dictionary<string, int> StartNodeVisitCounts { get; set; } = new Dictionary<string, int>();

		public static ActionGraphConfig FromJsonFile(string file)
		{
			return JsonConvert.DeserializeObject<ActionGraphConfig>(File.ReadAllText(file));
		}
	}

	public class ActionGraphFlagsConfig
	{
		public Dictionary<string, ActionNodeFlags> Nodes { get; set; } = new Dictionary<string, ActionNodeFlags>();

		public Dictionary<string, ConditionEdgeFlags> Edges { get; set; } = new Dictionary<string, ConditionEdgeFlags>();
	}
}
