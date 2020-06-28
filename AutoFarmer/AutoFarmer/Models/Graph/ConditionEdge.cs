using AutoFarmer.Models.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoFarmer.Models.Graph
{
	public class ConditionEdge
	{
		public string Name { get; set; }

		public string StartNodeName { get; set; }

		public string EndNodeName { get; set; }

		public List<Condition> Conditions { get; set; }

		public int MaxCrossing { get; set; } = 1;

		public int CurrentCrossing { get; set; }

		public int Order { get; set; } = 1;

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

		public bool ProcessConditions(out List<SerializablePoint> actionPoints)
		{
			actionPoints = new List<SerializablePoint>() { MouseSafetyMeasures.Instance.LastActionPosition };

			Logger.Log("Processing conditions");

			for (int i = 0; i < Conditions.Count; i++)
			{
				var result = Conditions[i].Process(out actionPoints);

				Logger.Log($"Condition processed: {result}");

				if (result == false)
				{
					if (i == 0)
					{
						return false;
					}
					else
					{
						throw new AutoFarmerException($"Stuck in {Name} condition edge!");
					}
				}
			}

			CurrentCrossing++;

			return true;
		}
	}
}
