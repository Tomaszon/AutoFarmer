using AutoFarmer.Models.Common;
using System;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public abstract class ConditionEdgeBase : FlagBase<ConditionEdgeFlags>
	{
		public int MaxCrossing { get; set; } = 1;

		public int Order { get; set; } = 1;

		public double ConsiderationProbability { get; set; } = 1;
	}
}
