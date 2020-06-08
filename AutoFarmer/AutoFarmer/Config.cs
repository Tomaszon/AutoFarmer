using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer
{
	public class Config
	{
		public MouseSafetyMeasures MouseSafetyMeasures { get; set; }

		public bool ActionSounds { get; set; }

		public string ScenarioConfigsRootDirectory { get; set; }

		public string AtomicScenariosDirectory
		{
			get
			{
				return Path.Combine(ScenarioConfigsRootDirectory, "atomicScenarioConfigs");
			}
		}

		public string GroupScenariosDirectory
		{
			get
			{
				return Path.Combine(ScenarioConfigsRootDirectory, "groupScenarioConfigs");
			}
		}

		public static Config FromJsonFile(string path)
		{
			return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
		}

		public int ProcessCountdown { get; set; }
	}
}
