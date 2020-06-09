using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace AutoFarmer
{
	public class Config
	{
		public MouseSafetyMeasures MouseSafetyMeasures { get; set; }

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

		public string ImageMatchTemplateResourcesDirectory { get; set; }

		public string TemplateFileExtension { get; set; }

		public static Config FromJsonFile(string path)
		{
			var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
			config.ConfigDirectory = Path.GetDirectoryName(path);

			return config;
		}

		public int ProcessCountdown { get; set; }
	}
}
