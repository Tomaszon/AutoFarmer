using AutoFarmer.Models.ImageMatching;
using System.Collections.Generic;
using System.Drawing;

namespace AutoFarmer.Models.GraphNamespace
{
	public class ConditionEdge
	{
		public string Name { get; set; }

		public string StartNodeName { get; set; }

		public string EndNodeName { get; set; }

		public Conditions Conditions { get; set; }

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

		public bool Process(out List<SerializablePoint> actionPoints)
		{
			var preRes = ProcessConditon(Conditions.PreCondition, out actionPoints);

			Logger.Log($"Precondition processed: {preRes}");

			if (Conditions.PreCondition?.Equals(Conditions.PostCondition) == true)
			{
				if (preRes) CurrentCrossing++;

				return preRes;
			}
			else
			{
				if (preRes is false) return false;

				var postRes = ProcessConditon(Conditions.PostCondition, out actionPoints);

				Logger.Log($"Postcondition processed: {postRes}");

				if (postRes)
				{
					CurrentCrossing++;

					return true;
				}
				else
				{
					throw new AutoFarmerException($"Stuck in {Name} condition edge!");
				}
			}
		}

		private bool ProcessConditon(MatchCondition condition, out List<SerializablePoint> actionPoints)
		{
			actionPoints = new List<SerializablePoint>() { MouseSafetyMeasures.Instance.LastActionPosition };

			if (condition is null) return true;

			return condition.Process(out actionPoints);
		}
	}
}
