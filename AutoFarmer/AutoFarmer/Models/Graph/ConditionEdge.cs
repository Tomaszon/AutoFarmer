using AutoFarmer.Models.Common;
using System.Collections.Generic;

namespace AutoFarmer.Models.Graph
{
	public class ConditionEdge
	{
		public string Name { get; set; }

		public string StartNodeName { get; set; }

		public string EndNodeName { get; set; }

		public Condition Condition { get; set; }

		public int MaxCrossing { get; set; }

		public int CurrentCrossing { get; set; }

		public int Order { get; set; }

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
