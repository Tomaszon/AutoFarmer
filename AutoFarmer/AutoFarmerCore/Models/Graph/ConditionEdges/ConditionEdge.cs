using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.Conditions;
using System.Collections.Generic;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public class ConditionEdge : ConditionEdgeBase
	{
		public string StartNodeName { get; set; }

		public string EndNodeName { get; set; }

		public Condition Condition { get; set; }

		public bool IsEnabled
		{
			get { return CurrentCrossing < MaxCrossing; }
		}

		public void Disable()
		{
			Logger.Log($"Edge {Name} disabled!");

			CurrentCrossing = int.MaxValue;
		}

		public void ResetState()
		{
			CurrentCrossing = 0;
		}

		public bool ProcessCondition(List<SerializablePoint> actionPoints)
		{
			Logger.Log($"Processing condition of {Name}");

			var result = Condition.Process(actionPoints);

			Logger.Log($"Condition processed: {result}");

			if (result)
			{
				CurrentCrossing++;

				return true;
			}

			return false;
		}
	}
}
