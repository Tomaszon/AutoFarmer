namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public abstract class ConditionEdgeBase
	{
		public string Name { get; set; }

		public int MaxCrossing { get; set; } = 1;

		public int CurrentCrossing { get; set; }

		public int Order { get; set; } = 1;

		public ReportMessages ReportMessages { get; set; }
	}
}
