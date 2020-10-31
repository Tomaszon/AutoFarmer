using AutoFarmer.Models.Common;
using System.Collections.Generic;

namespace AutoFarmer.Models.Graph
{
	public class ActionGraphFlagsConfig
	{
		public Dictionary<string, ActionNodeFlags> Nodes { get; set; } = new Dictionary<string, ActionNodeFlags>();

		public Dictionary<string, ConditionEdgeFlags> Edges { get; set; } = new Dictionary<string, ConditionEdgeFlags>();
	}
}
