using AutoFarmer.Services.Logging;
using AutoFarmer.Models.Common;

namespace AutoFarmer.Models.Graph.ActionNodes
{
	public class ActionNode : ActionNodeBase
	{
		public string Name { get; set; }

		public int CurrentCrossing { get; set; }

		public bool Crossable => CurrentCrossing < MaxCrossing;

		public void ResetState(bool complete)
		{
			if (complete || IsNot(ActionNodeFlags.StartNode))
			{
				CurrentCrossing = 0;
			}
		}

		public ActionNode(string name)
		{
			Name = name;
		}
	}
}
