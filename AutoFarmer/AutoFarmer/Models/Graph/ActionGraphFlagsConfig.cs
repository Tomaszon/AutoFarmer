using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AutoFarmer.Models.Graph
{
	public class ActionGraphFlagsConfig
	{
		public ActionGraphFlagsConfigRecords AddFlags { get; set; } = new ActionGraphFlagsConfigRecords();

		public ActionGraphFlagsConfigRecords RemoveFlags { get; set; } = new ActionGraphFlagsConfigRecords();

		public static ActionGraphFlagsConfig FromJsonFile(string file)
		{
			return JsonConvert.DeserializeObject<ActionGraphFlagsConfig>(File.ReadAllText(file));
		}
	}

	public class ActionGraphFlagsConfigRecords
	{
		public Dictionary<string, ActionNodeFlags> Nodes { get; set; } = new Dictionary<string, ActionNodeFlags>();

		public Dictionary<string, ConditionEdgeFlags> Edges { get; set; } = new Dictionary<string, ConditionEdgeFlags>();
	}
}
