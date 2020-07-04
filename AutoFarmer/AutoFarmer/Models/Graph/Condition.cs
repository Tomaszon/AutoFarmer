using AutoFarmer.Models.Common;
using AutoFarmer.Models.ImageMatching;
using System.Collections.Generic;
using System.Threading;

namespace AutoFarmer.Models.Graph
{
	public class Condition
	{
		public ResultAppendMode AppendMode { get; set; }

		public List<Condition> AndConditions { get; set; }

		public List<Condition> OrConditions { get; set; }

		public ConditionMode ConditionMode
		{
			get
			{
				if (AndConditions != null || OrConditions != null)
				{
					return AndConditions != null ? ConditionMode.And : ConditionMode.Or;
				}

				return ConditionMode.Primitive;
			}
		}

		public string TemplateName { get; set; }

		public string SearchRectangleName { get; set; }

		public int MaximumOccurrence { get; set; }

		public int MinimumOccurrence { get; set; }

		public int MaxRetryPerSimiliarityThreshold { get; set; }

		public int RetryDelay { get; set; }

		public bool DisableSearchAreaFallback { get; set; }

		public float MaximumSimiliarityThreshold { get; set; }

		public float MinimumSimiliarityThreshold { get; set; }

		public float SimiliarityThresholdStep { get; set; }

		public Dictionary<MatchOrderBy, MatchOrderLike> OrderBy { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			if (obj is Condition c)
			{
				return TemplateName == c.TemplateName && SearchRectangleName == c.SearchRectangleName;
			}

			return false;
		}

		public bool Process(List<SerializablePoint> actionPoints)
		{
			if (ConditionMode == ConditionMode.Primitive)
			{
				return ProcessPrimitiveCondition(actionPoints);
			}
			else
			{
				return ProcessComplexCondition(actionPoints);
			}
		}

		private bool ProcessComplexCondition(List<SerializablePoint> actionPoints)
		{
			var startState = ConditionMode == ConditionMode.And;

			foreach (var condition in AndConditions ?? OrConditions)
			{
				var lastProcessState = condition.Process(actionPoints);

				if (lastProcessState != startState)
				{
					return lastProcessState;
				}
			}

			return ConditionMode == ConditionMode.And;
		}

		private bool ProcessPrimitiveCondition(List<SerializablePoint> actionPoints)
		{
			float maximumThreshold = MaximumSimiliarityThreshold == default ? ImageMatchFinder.Instance.DefaultMaximumSimiliarityThreshold : MaximumSimiliarityThreshold;
			float minimumThreshold = MinimumSimiliarityThreshold == default ? ImageMatchFinder.Instance.DefaultMiniumuSimiliarityThreshold : MinimumSimiliarityThreshold;
			float thresholdStep = SimiliarityThresholdStep == default ? ImageMatchFinder.Instance.DefaultSimiliarityThresholdStep : SimiliarityThresholdStep;

			float current = maximumThreshold;

			Logger.Log($"Attempting to find {SearchRectangleName} search rectangle of {TemplateName} " +
				$"template max {MaxRetryPerSimiliarityThreshold + 1} times per similiarity threshold from: " +
				$"{maximumThreshold} to {minimumThreshold} with -{thresholdStep} steps");

			while (current >= minimumThreshold)
			{
				int retry = 0;

				while (retry <= MaxRetryPerSimiliarityThreshold)
				{
					try
					{
						MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

						var sourceImage = ImageFactory.CreateScreenshot();

						var foundActionPoints = ImageMatchFinder.FindClickPointForTemplate(this, sourceImage, current);

						switch (AppendMode)
						{
							case ResultAppendMode.Override:
							{
								actionPoints.Clear();

								actionPoints.AddRange(foundActionPoints);
							}
							break;

							case ResultAppendMode.Append:
							{
								actionPoints.AddRange(foundActionPoints);
							}
							break;
						}

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

				current -= thresholdStep;
			}

			return false;
		}
	}
}
