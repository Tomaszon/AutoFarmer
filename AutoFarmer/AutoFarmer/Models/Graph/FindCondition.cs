using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoFarmer.Models.GraphNamespace
{
	public class FindCondition
	{
		public string TemplateName { get; set; }

		public string SearchRectangleName { get; set; }

		public int MaximumOccurrence { get; set; } = 1;

		public int MinimumOccurrence { get; set; } = 1;

		public int MaxRetryPerSimiliarityThreshold { get; set; } = 1;

		public int RetryDelay { get; set; } = 1000;

		public float MaximumSimiliarityThreshold { get; set; } = 0.99f;

		public float MinimumSimiliarityThreshold { get; set; } = 0.98f;

		public float SimiliarityThresholdStep { get; set; } = 0.01f;

		public Dictionary<MatchOrderBy, MatchOrderLike> OrderBy { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			if (obj is FindCondition c)
			{
				return TemplateName == c.TemplateName && SearchRectangleName == c.SearchRectangleName;
			}

			return false;
		}

		public FindCondition Clone()
		{
			return JsonConvert.DeserializeObject<FindCondition>(JsonConvert.SerializeObject(this));
		}
	}
}
