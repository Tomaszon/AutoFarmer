using System.Collections.Generic;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public class ReportMessages
	{
		public Dictionary<string, string> Success { get; set; }

		public Dictionary<string, string> Fail { get; set; }
	}
}
