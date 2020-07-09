using AutoFarmer.Models.Common;
using AutoFarmer.Services.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static AutoFarmer.Models.Graph.IOptions;

namespace AutoFarmer.Models.Graph.Conditions
{
	public class ConditionOptions : ConditionBase, IOptions
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

		public ConditionOptions Clone()
		{
			return JsonConvert.DeserializeObject<ConditionOptions>(JsonConvert.SerializeObject(this));
		}

		public void ReplaceVariablesInCondition(Dictionary<string, List<object>> templateVariables, int index)
		{
			using var log = Logger.LogBlock();

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
				DisableSearchAreaFallback = DisableSearchAreaFallback,
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
