using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoFarmerCore.Models.Common
{
	public class ReportBuilder
	{
		public Dictionary<string, List<string>> MessageEntries { get; set; } = new Dictionary<string, List<string>>();

		public Dictionary<string, List<string>> MessageEntryBuffer { get; set; } = new Dictionary<string, List<string>>();

		public string ReportDirectory { get; set; }

		public bool GenerateReport { get; set; } = true;

		public bool SaveToFile { get; set; } = true;

		public static ReportBuilder Instance { get; set; }

		public static void Add(string key, string value, bool toBuffer = true)
		{
			var selectedDictionary = toBuffer ? Instance.MessageEntryBuffer : Instance.MessageEntries;

			if (selectedDictionary.ContainsKey(key))
			{
				selectedDictionary[key].Add(value);
			}
			else
			{
				selectedDictionary.Add(key, new List<string>(new[] { value }));
			}
		}

		public static void CommitBuffer()
		{
			foreach (var t in Instance.MessageEntryBuffer)
			{
				foreach (var v in t.Value)
				{
					Add(t.Key, v, false);
				}
			}

			ClearBuffer();
		}

		public static void ClearBuffer()
		{
			Instance.MessageEntryBuffer.Clear();
		}

		public static void Clear()
		{
			Instance.MessageEntries.Clear();
		}

		public static void Generate()
		{
			if (Instance.GenerateReport)
			{
				using var log = Logger.LogBlock();

				if (Instance.MessageEntries.Count > 0)
				{
					StringBuilder result = new StringBuilder();

					foreach (var t in Instance.MessageEntries)
					{
						if (t.Value.Count > 1)
						{
							result.Append($"{t.Key}:\n");

							foreach (var e in t.Value)
							{
								result.Append($"\t{e}\n");
							}
						}
						else
						{
							result.Append($"{t.Key}: {t.Value.First()}\n");
						}

						result.Append("\n");
					}

					Directory.CreateDirectory(Instance.ReportDirectory);

					var report = result.ToString().Trim();

					Console.WriteLine(report);

					if (Instance.SaveToFile)
					{
						File.AppendAllText(Path.Combine(Instance.ReportDirectory, $"{Logger.Instance.SessionId}.report"), report);
					}
				}
			}
		}

		public static void FromJsonFile(string path)
		{
			Instance = JsonConvert.DeserializeObject<ReportBuilder>(File.ReadAllText(path));

			Instance.ReportDirectory = Instance.ReportDirectory ?? Path.Combine(Directory.GetParent(Config.Instance.ConfigDirectory).FullName, "Reports");
		}
	}
}
