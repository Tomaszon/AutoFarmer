using AutoFarmer.Models.Common;
using AutoFarmer.Services;
using AutoFarmer.Services.Imaging;
using AutoFarmer.Services.Logging;
using AutoFarmer.Services.ReportBuilder;
using System.Collections.Generic;
using System.Threading;

namespace AutoFarmer.Models.Graph.Conditions
{
	public class Condition : ConditionBase
	{
		public ResultAppendMode AppendMode { get; set; }

		public List<Condition>? AndConditions { get; set; }

		public List<Condition>? OrConditions { get; set; }

		public ConditionMode ConditionMode
		{
			get
			{
				if (AndConditions is { } || OrConditions is { })
				{
					return AndConditions is { } ? ConditionMode.And : ConditionMode.Or;
				}

				return ConditionMode.Primitive;
			}
		}

		public Condition(string templateName, string searchRectangleName)
		{
			TemplateName = templateName;
			SearchRectangleName = searchRectangleName;
		}

		public override bool Equals(object? obj)
		{
			if (obj is null) return false;

			if (obj is Condition c)
			{
				return TemplateName.Equals(c.TemplateName) && SearchRectangleName.Equals(c.SearchRectangleName);
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

			foreach (var condition in AndConditions ?? OrConditions!)
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

						if (ReportMessages is { })
						{
							ReportBuilder.AddRange(ReportMessages.Success, ReportMessageType.Success);
						}

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

			if (ReportMessages is { })
			{
				ReportBuilder.AddRange(ReportMessages.Fail, ReportMessageType.Fail);
			}

			return false;
		}
	}
}
