﻿namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public abstract class ConditionEdgeBase
	{
		public int MaxCrossing { get; set; } = 1;

		public int CurrentCrossing { get; set; }

		public int Order { get; set; } = 1;

		public double ConsiderationProbability { get; set; } = 1;
	}
}
