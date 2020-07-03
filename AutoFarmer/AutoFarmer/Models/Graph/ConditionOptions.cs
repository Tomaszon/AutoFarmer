﻿using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AutoFarmer.Models.Graph
{
	public class ConditionOptions : Options
	{
		public ResultAppendMode Result { get; set; }

		public List<ConditionOptions> And { get; set; }

		public List<ConditionOptions> Or { get; set; }

		public ConditionMode Mode
		{
			get
			{
				if (And != null || Or != null)
				{
					return And != null ? ConditionMode.And : ConditionMode.Or;
				}

				return ConditionMode.Primitive;
			}
		}

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
					TemplateName = ReplaceVariables(TemplateName, templateVariables, index);
					SearchRectangleName = ReplaceVariables(SearchRectangleName, templateVariables, index);
				}
				break;

				case ConditionMode.Or:
				{
					foreach (var condition in Or)
					{
						condition.ReplaceVariablesInCondition(templateVariables, index);
					}
				}
				break;

				case ConditionMode.And:
				{
					foreach (var condition in And)
					{
						condition.ReplaceVariablesInCondition(templateVariables, index);
					}
				}
				break;
			}
		}

		public Condition ToCondition()
		{
			return new Condition()
			{
				AndConditions = And?.Select(c => c.ToCondition()).ToList(),
				AppendMode = Result,
				MaximumOccurrence = MaximumOccurrence,
				MaximumSimiliarityThreshold = MaximumSimiliarityThreshold,
				MaxRetryPerSimiliarityThreshold = MaxRetryPerSimiliarityThreshold,
				MinimumOccurrence = MinimumOccurrence,
				MinimumSimiliarityThreshold = MinimumSimiliarityThreshold,
				OrConditions = Or?.Select(c => c.ToCondition()).ToList(),
				OrderBy = OrderBy,
				RetryDelay = RetryDelay,
				SearchRectangleName = SearchRectangleName,
				SimiliarityThresholdStep = SimiliarityThresholdStep,
				TemplateName = TemplateName
			};
		}
	}
}