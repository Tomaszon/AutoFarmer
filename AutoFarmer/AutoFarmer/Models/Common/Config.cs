using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer.Models.Common
{
	public class Config
	{
		public SerializableSize ScreenSize { get; set; }

		public bool ActionSounds { get; set; }

		public string ConfigDirectory { get; set; }

		public string ActionNodesDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "actionNodes");
			}
		}

		public string ConditionEdgesDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "conditionEdges");
			}
		}

		public string ImageMatchTemplatesDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "imageMatchTemplates");
			}
		}

		public string ImageMatchFinderConfigPath
		{
			get
			{
				return Path.Combine(ConfigDirectory, "imageMatchFinderConfig.json");
			}
		}

		public string MouseSafetyMeasuresConfigPath
		{
			get
			{
				return Path.Combine(ConfigDirectory, "mouseSafetyMeasuresConfig.json");
			}
		}

		public string ImageMatchTemplateResourcesDirectory { get; set; }

		public int ProcessCountdown { get; set; }

		public string LogDirectory
		{
			get
			{
				return Path.Combine(Directory.GetParent(ConfigDirectory).FullName, "Logs");
			}

		}

		public bool FileLogging { get; set; }

		public string[] ActiveStartNodes { get; set; }

		public bool GraphicalLogging { get; set; }

		public static Config Instance { get; set; }

		public static void FromJsonFile(string path)
		{
			Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
			Instance.ConfigDirectory = Path.GetDirectoryName(path);
		}
	}
}
