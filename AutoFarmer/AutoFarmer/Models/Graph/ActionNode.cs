namespace AutoFarmer.Models.Graph
{
	public class ActionNode
	{
		public string Name { get; set; }

		public string[] ActionNames { get; set; }

		public int AdditionalDelayBetweenActions { get; set; }

		public int AdditionalDelayAfterLastAction { get; set; }

		public bool IsStartNode { get; set; }

		public bool IsEndNode { get; set; }

		public bool IsVisited { get; set; }

		public void ResetState()
		{
			if (!IsStartNode)
			{
				IsVisited = false;
			}
		}
	}
}
