using System.Collections.Generic;

namespace AutoFarmer.Models.Graph.ConditionEdges
{
	public class ReportMessages
	{
		public Dictionary<string, string> Success { get; set; } = new Dictionary<string, string>();

		public Dictionary<string, string> Fail { get; set; } = new Dictionary<string, string>();
	}
}
