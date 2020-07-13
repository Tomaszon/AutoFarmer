using AutoFarmer.Services.Logging;

namespace AutoFarmer.Models.Graph.ActionNodes
{
	public class ActionNode : ActionNodeBase
	{
		public string Name { get; set; }

		public void ResetState(bool complete)
		{
			if (complete || !IsStartNode)
			{
				IsVisited = false;
			}
		}
	}
}
