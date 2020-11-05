using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.Conditions;
using AutoFarmer.Services.Logging;
using System.Collections.Generic;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public class ConditionEdge : ConditionEdgeBase
	{
		public int PreferredOrder { get; set; }

		public string Name => $"{StartNodeName}-{EndNodeName}";

		public int CurrentCrossing { get; set; }

		public bool Crossable => CurrentCrossing < MaxCrossing;

		public string StartNodeName { get; set; }

		public string EndNodeName { get; set; }

		public Condition? Condition { get; set; }

		public ConditionEdge(string startNodeName, string endNodeName, int order, int preferredOrder, Condition? condition, int maxCrossing, double considerationProbability, ConditionEdgeFlags? flags)
		{
			StartNodeName = startNodeName;
			EndNodeName = endNodeName;
			Order = order;
			PreferredOrder = preferredOrder;
			MaxCrossing = maxCrossing;
			ConsiderationProbability = considerationProbability;
			Condition = condition;
			Flags = flags;
		}

		public void ResetState(bool complete)
		{
			if (complete || IsNot(ConditionEdgeFlags.Switch))
			{
				CurrentCrossing = 0;
				RemoveFlags(ConditionEdgeFlags.Tried);
			}
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
