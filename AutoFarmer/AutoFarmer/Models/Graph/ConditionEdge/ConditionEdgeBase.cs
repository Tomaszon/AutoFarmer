namespace AutoFarmer.Models.Graph
{
	public abstract class ConditionEdgeBase
	{
		public string Name { get; set; }

		public int MaxCrossing { get; set; } = 1;

		public int CurrentCrossing { get; set; }

		public int Order { get; set; } = 1;
	}
}
