using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AutoFarmer.Services
{
	public class GlobalStateStorage
	{
		public Dictionary<string, object> Storage { get; set; }

		public static GlobalStateStorage Instance { get; set; }

		public static T Get<T>(string variableName)
		{
			return (T)Instance.Storage[variableName];
		}

		public static void FromJsonFile(string path)
		{
			Instance = JsonConvert.DeserializeObject<GlobalStateStorage>(File.ReadAllText(path));
		}
	}
}
