using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer.Services
{
	public class Config
	{
		//[DllImport("user32.dll")]
		//private static extern bool GetCursorPos(out Point lpPoint);

		//TODO get the value automatically with System.Windows.Forms.dll
		//TODO update to handle multi-monitor setups
		public SerializableSize ScreenSize
		{
			get;
			//{
			//GetCursorPos(out var p);
			//return (SerializableSize)Screen.FromPoint(p).Bounds.Size;
			//}
			set;
		} = null!;

		public SerializablePoint ScreenLocation { get; set; } = new SerializablePoint();

		public bool ActionSounds { get; set; }

		public string ConfigDirectory { get; set; } = null!;

		public string ActionNodesDirectory
		{
			get
			{
				return Path.Combine(ActivePackageDirectory, "actionNodes");
			}
		}

		public string ConditionEdgesDirectory
		{
			get
			{
				return Path.Combine(ActivePackageDirectory, "conditionEdges");
			}
		}

		public string ImageMatchTemplatesDirectory
		{
			get
			{
				return Path.Combine(ActivePackageDirectory, "imageMatchTemplates");
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

		public string GlobalStateStorageConfigPath
		{
			get
			{
				return Path.Combine(ActivePackageDirectory, "globalStateStorageConfig.json");
			}
		}

		public string ActionGraphConfigPath
		{
			get
			{
				return Path.Combine(ActivePackageDirectory, "actionGraphFlagsConfig.json");
			}
		}

		public string ImageMatchTemplateResourcesDirectory
		{
			get
			{
				return Path.Combine(ActivePackageDirectory, "Templates");
			}
		}

		public string ActivePackageDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "Packages", ActivePackage);
			}
		}

		public int ProcessCountdown { get; set; }

		public string ActivePackage { get; set; } = null!;

		public static Config Instance { get; set; } = null!;

		public static void FromJsonFile(string path)
		{
			Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
			Instance.ConfigDirectory = Path.GetDirectoryName(path)!;
		}
	}
}
