using AutoFarmer.Models.ImageMatching;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

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

		public bool Process(out List<SerializablePoint> actionPoints)
		{
			actionPoints = new List<SerializablePoint>() { MouseSafetyMeasures.Instance.LastActionPosition };

			float maximum = MaximumSimiliarityThreshold == default ? ImageMatchFinder.Instance.DefaultMaximumSimiliarityThreshold : MaximumSimiliarityThreshold;
			float minimum = MinimumSimiliarityThreshold == default ? ImageMatchFinder.Instance.DefaultMiniumuSimiliarityThreshold : MinimumSimiliarityThreshold;
			float step = SimiliarityThresholdStep == default ? ImageMatchFinder.Instance.DefaultSimiliarityThresholdStep : SimiliarityThresholdStep;

			float current = maximum;

			Logger.Log($"Attempting to find {SearchRectangleName} search rectangle of {TemplateName} " +
				$"template max {MaxRetryPerSimiliarityThreshold + 1} times per similiarity threshold from: " +
				$"{maximum} to {minimum} with -{step} steps");

			while (current >= minimum)
			{
				int retry = 0;

				while (retry <= MaxRetryPerSimiliarityThreshold)
				{
					try
					{
						MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

						var sourceImage = ScreenshotMaker.CreateScreenshot();

						actionPoints = ImageMatchFinder.FindClickPointForTemplate(this, sourceImage, current);

						MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

						return true;
					}
					catch (ImageMatchNotFoundException)
					{
						retry++;

						Logger.Log($"Match not found for the {retry}. time with {current} similiarity threshold!", NotificationType.Error);

						Thread.Sleep(RetryDelay);
					}
					catch (ImageMatchAmbiguousException ex)
					{
						throw new AutoFarmerException("Automatic emergency stop!", ex);
					}
				}

				current -= step;
			}

			return false;
		}
	}
}
