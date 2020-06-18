namespace AutoFarmer.Models.GraphNamespace
{
	public class ActionNode
	{
		public string Name { get; set; }

		public NodeActions Actions { get; set; }

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
