using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AutoFarmer.Services
{
	public class GlobalStateStorage
	{
		public Dictionary<string, GlobalStateStorageVariable> Variables { get; set; } = new Dictionary<string, GlobalStateStorageVariable>();

		public static GlobalStateStorage Instance { get; set; }

		public static GlobalStateStorageVariable Get(string variableName)
		{
			return Instance.Variables[variableName];
		}

		public static void Reset()
		{
			foreach (var t in Instance.Variables)
			{
				t.Value.Reset();
			}
		}

		public static void FromConfig()
		{
			Instance = JsonConvert.DeserializeObject<GlobalStateStorage>(File.ReadAllText(Config.Instance.GlobalStateStorageConfigPath));
			Reset();
		}
	}
}
