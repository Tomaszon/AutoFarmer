using AutoFarmer.Services.Logging;

namespace AutoFarmer.Models.Graph.ActionNodes
{
	public class ActionNode : ActionNodeBase
	{
		public string Name { get; set; }

		public void ResetState()
		{
			using var log = Logger.LogBlock();

			if (!IsStartNode)
			{
				IsVisited = false;
			}
		}
	}
}
