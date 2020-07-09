namespace AutoFarmer.Services.Logging
{
	public abstract class LoggerBase
	{
		public bool FileLogging { get; set; }

		public bool GraphicalLogging { get; set; }

		public string LogDirectory { get; set; }
	}
}
