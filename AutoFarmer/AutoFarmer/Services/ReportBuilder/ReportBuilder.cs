using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Services.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static AutoFarmer.Models.Graph.IOptions;

namespace AutoFarmer.Services.ReportBuilder
{
	public class ReportBuilder : IOptions
	{
		public string ReportDirectory { get; set; }

		public bool GenerateReport { get; set; } = true;

		public bool SaveToFile { get; set; } = true;

		public static ReportBuilder Instance { get; set; }

		public ReportBuilderMessageContainer Container { get; set; } = new ReportBuilderMessageContainer();

		public static void AddRange(Dictionary<string, string> keyValuePairs, ReportMessageType type)
		{
			using var log = Logger.LogBlock();

			foreach (var t in keyValuePairs)
			{
				Add(t.Key, t.Value, type);
			}
		}

		public static void Add(string key, string value, ReportMessageType type)
		{
			using var log = Logger.LogBlock();

			Instance.Container.AddToBuffer(key, type, value);
		}

		public static void Commit(ReportMessageType type)
		{
			using var log = Logger.LogBlock();

			Instance.Container.Commit(type);
		}

		//TODO
		public static void Generate()
		{
			if (Instance.GenerateReport)
			{
				using var log = Logger.LogBlock();

				if (Instance.Container.Messages.Count > 0)
				{
					StringBuilder result = new StringBuilder();

					foreach (var t in Instance.Container.Messages)
					{
						if (t.Value.Count > 1)
						{
							result.Append($"{t.Key}:\n");

							foreach (var e in t.Value)
							{
								result.Append($"  {e}\n");
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

					Console.WriteLine(report + "\n");

					if (Instance.SaveToFile)
					{
						File.AppendAllText(Path.Combine(Instance.ReportDirectory, $"{Logger.Instance.SessionId}.log"), report);
					}

					Instance.Container.Clear();
				}
			}
		}

		public static void FromJsonFileWithConfig(string path)
		{
			FromJsonFileWrapper(() =>
			{
				Instance = JsonConvert.DeserializeObject<ReportBuilder>(File.ReadAllText(path));

				Instance.ReportDirectory ??= Path.Combine(Directory.GetParent(Config.Instance.ConfigDirectory).FullName, "Reports");
			});
		}
	}
}
