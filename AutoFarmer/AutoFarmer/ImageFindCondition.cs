namespace AutoFarmer
{
	public class ImageFindCondition
	{
		public string TemplateName { get; set; }

		public string SearchRectangleName { get; set; }

		public int MaxAmbiguousity { get; set; } = 1;

		public int MaxRetry { get; set; } = 1;

		public int RetryDelay { get; set; } = 1000;

		public float MaximumSimiliarityThreshold { get; set; } = 1;

		public float MinimumSimiliarityThreshold { get; set; } = 0.98f;

		public float SimiliarityThresholdStep { get; set; } = 0.01f;

		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			if (obj is ImageFindCondition c)
			{
				return TemplateName == c.TemplateName && SearchRectangleName == c.SearchRectangleName;
			}

			return false;
		}
	}
}
