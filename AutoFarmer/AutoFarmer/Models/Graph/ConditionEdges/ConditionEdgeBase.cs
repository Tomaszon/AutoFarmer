using AutoFarmer.Models.Common;
using System;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public abstract class ConditionEdgeBase
	{
		public int MaxCrossing { get; set; } = 1;

		public int Order { get; set; } = 1;

		public double ConsiderationProbability { get; set; } = 1;

		public ConditionEdgeFlags Flags { get; set; }

		public bool Is(ConditionEdgeFlags flags)
		{
			return ((int)Flags & (int)flags) == (int)flags;
		}

		public bool IsNot(ConditionEdgeFlags flags)
		{
			return !Is(flags);
		}

		public void AddFlags(ConditionEdgeFlags flags)
		{
			Flags |= flags;
		}

		public void RemoveFlags(ConditionEdgeFlags flags)
		{
			Flags &= ~flags;
		}
	}
}
