using System.Collections.Generic;

namespace AutoFarmer.Models.Graph.ActionNodes
{
	public abstract class ActionNodeBase
	{
		public Dictionary<string, List<object>> TemplateVariables { get; set; }

		public string[] Names { get; set; }

		public string[] ActionNames { get; set; }

		public int AdditionalDelayBetweenActions { get; set; }

		public int AdditionalDelayAfterLastAction { get; set; } = 500;

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; set; }

		public bool IsVisited { get; set; }
	}
}
