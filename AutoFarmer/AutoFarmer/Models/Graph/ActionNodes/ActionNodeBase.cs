namespace AutoFarmer.Models.Graph.ActionNodes
{
	public abstract class ActionNodeBase
	{
		public string[] ActionNames { get; set; }

		public int AdditionalDelayBetweenActions { get; set; }

		public int AdditionalDelayAfterLastAction { get; set; } = 1000;

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; set; }

		public bool IsVisited { get; set; }
	}
}
