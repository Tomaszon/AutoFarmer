using System.Collections.Generic;

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
			CurrentCrossing = int.MaxValue;
		}

		public void ResetState()
		{
			CurrentCrossing = 0;
		}

		public static List<ConditionEdge> FromOptions(ConditionEdgeOptions options)
		{
			List<ConditionEdge> result = new List<ConditionEdge>();

			foreach (var endPoint in options.Nodes)
			{
				result.Add(new ConditionEdge()
				{
					Conditions = options.Conditions,
					CurrentCrossing = options.CurrentCrossing,
					MaxCrossing = options.MaxCrossing,
					Order = options.Order,
					StartNodeName = endPoint.Key,
					EndNodeName = endPoint.Value,
					Name = $"{endPoint.Key}-{endPoint.Value}"
				});
			}

			return result;
		}
	}
}
