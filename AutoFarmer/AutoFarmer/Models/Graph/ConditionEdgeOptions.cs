using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AutoFarmer.Models.GraphNamespace
{
	public class ConditionEdgeOptions
	{
		public string Name { get; set; }

		public Dictionary<string, string> Nodes { get; set; }

		public Conditions Conditions { get; set; }

		public int MaxCrossing { get; set; } = 1;

		public int CurrentCrossing { get; set; }

		public int Order { get; set; } = 1;

		public static ConditionEdgeOptions FromJsonFile(string path)
		{
			var edgeOptions = JsonConvert.DeserializeObject<ConditionEdgeOptions>(File.ReadAllText(path));
			edgeOptions.Name = Path.GetFileNameWithoutExtension(path);

			if (edgeOptions.Nodes is null)
			{
				var arr = edgeOptions.Name.Split('-');

				edgeOptions.Nodes = new Dictionary<string, string>() { { arr[0], arr[1] } };
			}
			return edgeOptions;
		}
	}
}
