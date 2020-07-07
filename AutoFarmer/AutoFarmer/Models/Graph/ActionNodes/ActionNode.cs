using AutoFarmer.Models.Common;

namespace AutoFarmer.Models.Graph.ActionNodes
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
