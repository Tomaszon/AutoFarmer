using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer.Models.GraphNamespace
{
	public class ConditionEdge
	{
		public string Name { get; set; }

		public string StartNodeName { get; set; }

		public string EndNodeName { get; set; }

		public Conditions Conditions { get; set; }

		public int MaxCrossing { get; set; } = 1;

		public int CurrentCrossing { get; set; }

		public int Order { get; set; }

		public bool IsEnabled
		{
			get { return CurrentCrossing < MaxCrossing; }
		}

		public void Disable()
		{
			CurrentCrossing = int.MaxValue;
		}

		public void ResetState()
		{
			CurrentCrossing = 0;
		}

		public static ConditionEdge FromJsonFile(string path)
		{
			var edge = JsonConvert.DeserializeObject<ConditionEdge>(File.ReadAllText(path));
			edge.Name = Path.GetFileNameWithoutExtension(path);

			var arr = edge.Name.Split('-');

			edge.StartNodeName = arr[0];
			edge.EndNodeName = arr[1];

			return edge;
		}
	}
}
