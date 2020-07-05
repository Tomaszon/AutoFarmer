namespace AutoFarmer.Models.Graph
{
	public class ActionNode : ActionNodeBase
	{
		public string Name { get; set; }

		public void ResetState()
		{
			if (!IsStartNode)
			{
				IsVisited = false;
			}
		}
	}
}
