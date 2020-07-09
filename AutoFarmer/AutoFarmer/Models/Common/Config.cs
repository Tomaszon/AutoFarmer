﻿using Newtonsoft.Json;
using System.IO;

namespace AutoFarmer.Models.Common
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
		}

		public bool ActionSounds { get; set; }

		public string ConfigDirectory { get; set; }

		public string ActionNodesDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "Packages", ActivePackage, "actionNodes");
			}
		}

		public string ConditionEdgesDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "Packages", ActivePackage, "conditionEdges");
			}
		}

		public string ImageMatchTemplatesDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "Packages", ActivePackage, "imageMatchTemplates");
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

		public string ImageMatchTemplateResourcesDirectory
		{
			get
			{
				return Path.Combine(ConfigDirectory, "Packages", ActivePackage, "Templates");
			}
		}

		public int ProcessCountdown { get; set; }

		public string ActivePackage { get; set; }

		public string[] ActiveStartNodes { get; set; }

		public static Config Instance { get; set; }

		public static void FromJsonFile(string path)
		{
			Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
			Instance.ConfigDirectory = Path.GetDirectoryName(path);
		}
	}
}
