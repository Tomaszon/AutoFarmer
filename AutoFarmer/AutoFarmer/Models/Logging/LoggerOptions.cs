using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System.IO;

namespace AutoFarmerCore.Models.Common
{
	public class LoggerOptions : LoggerBase
	{
		public LogFormatterOptions Format { get; set; }

		public static Logger FromJsonFile(string path)
		{
			var loggerOptions = JsonConvert.DeserializeObject<LoggerOptions>(File.ReadAllText(path));

			return loggerOptions.ToLogger();
		}

		public Logger ToLogger()
		{
			return new Logger()
			{
				FileLogging = FileLogging,
				GraphicalLogging = GraphicalLogging,
				LogDirectory = LogDirectory ?? Path.Combine(Directory.GetParent(Config.Instance.ConfigDirectory).FullName, "Logs"),
				Formatter = Format.ToLogFormatter()
			};
		}
	}
}
