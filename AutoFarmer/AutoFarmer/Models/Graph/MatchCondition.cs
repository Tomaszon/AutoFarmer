using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoFarmer.Models.GraphNamespace
{
	public class MatchCondition
	{
		public string TemplateName { get; set; }

		public string SearchRectangleName { get; set; }

		public int MaximumOccurrence { get; set; } = 1;

		public int MinimumOccurrence { get; set; } = 1;

		public int MaxRetryPerSimiliarityThreshold { get; set; } = 1;

		public int RetryDelay { get; set; } = 1000;

		public float MaximumSimiliarityThreshold { get; set; }

		public float MinimumSimiliarityThreshold { get; set; }

		public float SimiliarityThresholdStep { get; set; }

		public Dictionary<MatchOrderBy, MatchOrderLike> OrderBy { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			if (obj is MatchCondition c)
			{
				return TemplateName == c.TemplateName && SearchRectangleName == c.SearchRectangleName;
			}

			return false;
		}

		public MatchCondition Clone()
		{
			return JsonConvert.DeserializeObject<MatchCondition>(JsonConvert.SerializeObject(this));
		}
	}
}
