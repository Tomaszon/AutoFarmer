using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.Conditions;
using AutoFarmer.Services.Logging;
using System.Collections.Generic;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public class ConditionEdge : ConditionEdgeBase
	{
		public string Name { get; set; }

		public string StartNodeName { get; set; }

		public string EndNodeName { get; set; }

		public Condition? Condition { get; set; }

		public bool IsEnabled
		{
			get { return CurrentCrossing < MaxCrossing; }
		}

		public ConditionEdge(string startNodeName, string endNodeName, int order, Condition? condition, int maxCrossing, double considerationProbability)
		{
			StartNodeName = startNodeName;
			EndNodeName = endNodeName;
			Name = $"{startNodeName}-{endNodeName}";
			Order = order;
			MaxCrossing = maxCrossing;
			ConsiderationProbability = considerationProbability;
			Condition = condition;
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

			if (Condition is null)
			{
				actionPoints.Clear();

				return true;
			}

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
