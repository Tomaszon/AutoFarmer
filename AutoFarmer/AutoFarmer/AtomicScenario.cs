using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer
{
	public class AtomicScenario
	{
		public string Name { get; set; }

		public string TemplateName { get; set; }

		public string SearchRectangleName { get; set; }

		public Actions Actions { get; set; }

		public static AtomicScenario FromJsonFile(string path)
		{
			var scenario = JsonConvert.DeserializeObject<AtomicScenario>(File.ReadAllText(path));
			scenario.Name = Path.GetFileNameWithoutExtension(path);

			if (scenario.Actions.Fail?.Retry != null && scenario.Actions.Fail?.Fallback != null)
			{
				throw new Exception($"Both Retry and Fallback actions configured in {scenario.Name}!");
			}

			return scenario;
		}

		public static List<AtomicScenario> LoadScenarios(string directory)
		{
			List<AtomicScenario> result = new List<AtomicScenario>();

			foreach (string fileName in Directory.GetFiles(directory))
			{
				result.Add(FromJsonFile(fileName));
			}

			foreach (var s in result)
			{
				if (s.Actions.Fail?.Fallback != null)
				{
					s.Actions.Fail.Fallback.Scenario = result.First(r => r.Name == s.Actions.Fail.Fallback.ScenarioName);
				}
			}

			return result;
		}
	}
}
