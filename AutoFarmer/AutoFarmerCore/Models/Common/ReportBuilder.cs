using AutoFarmer.Models.Common;
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

		public string ReportDirectory { get; set; }

		public static ReportBuilder Instance { get; set; }

		public static void Add(string key, string value)
		{
			if (Instance.MessageEntries.ContainsKey(key))
			{
				Instance.MessageEntries[key].Add(value);
			}
			else
			{
				Instance.MessageEntries.Add(key, new List<string>(new[] { value }));
			}
		}

		public static void Clear()
		{
			Instance.MessageEntries.Clear();
		}

		public static void Generate()
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

				File.AppendAllText(Path.Combine(Instance.ReportDirectory, $"{Logger.Instance.SessionId}.report"), result.ToString().Trim());
			}
		}

		public static void FromConfig()
		{
			Instance = new ReportBuilder()
			{
				ReportDirectory = Path.Combine(Directory.GetParent(Config.Instance.ConfigDirectory).FullName, "Report")
			};
		}
	}
}
