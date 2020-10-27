using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.ConditionEdges;

namespace AutoFarmer.Models.Graph.ActionNodes
{
	public abstract class ActionNodeBase : FlagBase<ActionNodeFlags>
	{
		public string[]? Actions { get; set; }

		public int AdditionalDelayBetweenActions { get; set; }

		public int AdditionalDelayAfterLastAction { get; set; } = 1000;

		public int MaxCrossing { get; set; } = 1;
	}
}
