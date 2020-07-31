using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.ConditionEdges;
using System.Collections.Generic;

namespace AutoFarmer.Models.Graph.Conditions
{
	public class ConditionBase
	{
		public string TemplateName { get; set; } = null!;

		public string SearchRectangleName { get; set; } = null!;

		public int MaximumOccurrence { get; set; } = 1;

		public int MinimumOccurrence { get; set; } = 1;

		public int MaxRetryPerSimiliarityThreshold { get; set; } = 1;

		public int RetryDelay { get; set; } = 1000;

		public bool DisableSearchAreaFallback { get; set; }

		public float MaximumSimiliarityThreshold { get; set; }

		public float MinimumSimiliarityThreshold { get; set; }

		public float SimiliarityThresholdStep { get; set; }

		public Dictionary<MatchOrderBy, MatchOrderLike>? OrderBy { get; set; }

		public ReportMessages? ReportMessages { get; set; }
	}
}
