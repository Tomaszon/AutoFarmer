﻿using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static AutoFarmer.Models.Graph.IOptions;

namespace AutoFarmer.Models.Graph.Conditions
{
	public class ConditionOptions : ConditionBase, IOptions
	{
		public ResultAppendMode Result { get; set; }

		public List<ConditionOptions>? And { get; set; }

		public List<ConditionOptions>? Or { get; set; }

		public ConditionMode Mode
		{
			get
			{
				if (And is { } || Or is { })
				{
					return And is { } ? ConditionMode.And : ConditionMode.Or;
				}

				return ConditionMode.Primitive;
			}
		}

		public ConditionOptions Clone()
		{
			return JsonConvert.DeserializeObject<ConditionOptions>(JsonConvert.SerializeObject(this));
		}

		public void ReplaceVariablesInCondition(Dictionary<string, List<object>> templateVariables, int index)
		{
			switch (Mode)
			{
				case ConditionMode.Primitive:
				{
					TemplateName = ReplaceVariables(TemplateName!, templateVariables, index);
					SearchRectangleName = ReplaceVariables(SearchRectangleName!, templateVariables, index);

					if (ReportMessages is { })
					{
						ReportMessages.Success = ReportMessages.Success.Select(t =>
							new KeyValuePair<string, string>(t.Key, ReplaceVariables(t.Value, templateVariables, index))).ToDictionary(t =>
								t.Key, t => t.Value);

						ReportMessages.Fail = ReportMessages.Fail.Select(t =>
							new KeyValuePair<string, string>(t.Key, ReplaceVariables(t.Value, templateVariables, index))).ToDictionary(t =>
								t.Key, t => t.Value);
					}
				}
				break;

				case ConditionMode.Or:
				{
					foreach (var condition in Or!)
					{
						condition.ReplaceVariablesInCondition(templateVariables, index);
					}
				}
				break;

				case ConditionMode.And:
				{
					foreach (var condition in And!)
					{
						condition.ReplaceVariablesInCondition(templateVariables, index);
					}
				}
				break;
			}
		}

		public Condition ToCondition()
		{
			return new Condition(TemplateName, SearchRectangleName)
			{
				AndConditions = And?.Select(c => c.ToCondition()).ToList(),
				AppendMode = Result,
				DisableSearchAreaFallback = DisableSearchAreaFallback,
				MaximumOccurrence = MaximumOccurrence,
				MaximumSimiliarityThreshold = MaximumSimiliarityThreshold,
				MaxRetryPerSimiliarityThreshold = MaxRetryPerSimiliarityThreshold,
				MinimumOccurrence = MinimumOccurrence,
				MinimumSimiliarityThreshold = MinimumSimiliarityThreshold,
				OrConditions = Or?.Select(c => c.ToCondition()).ToList(),
				OrderBy = OrderBy,
				RetryDelay = RetryDelay,
				SimiliarityThresholdStep = SimiliarityThresholdStep,
				ReportMessages = ReportMessages
			};
		}
	}
}
