using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Services
{
	public class GlobalStateStorage
	{
		public Dictionary<string, GlobalStateStorageValue> Values { get; set; } = new Dictionary<string, GlobalStateStorageValue>();

		public static GlobalStateStorage Instance { get; set; } = null!;

		public static GlobalStateStorageValue Get(string variableName)
		{
			return Instance.Values[variableName];
		}

		public static void Reset()
		{
			foreach (var t in Instance.Values)
			{
				t.Value.Reset();
			}
		}

		public static void FromConfig()
		{
			Instance = JsonConvert.DeserializeObject<GlobalStateStorage>(File.ReadAllText(Config.Instance.GlobalStateStorageConfigPath));
			Reset();
		}

		public static IEnumerable<string> GetValueNames()
		{
			return Instance.Values.Select(v => v.Key);
		}

		public static IEnumerable<decimal> GetValues()
		{
			return Instance.Values.Select(v => v.Value.Value);
		}
	}
}
