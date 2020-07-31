using System;
using System.Runtime.CompilerServices;

namespace AutoFarmer.Services.Logging
{
	public class LogBlock : IDisposable
	{
		private readonly string _name;

		private readonly string _file;

		private readonly int _line;

		public LogBlock(string? name = null, [CallerFilePath] string file = null!, [CallerMemberName] string method = null!, [CallerLineNumber] int line = default)
		{
			_name = $"{method}{(name is { } ? $": {name}" : "")}";

			_line = line;

			_file = file;

			Logger.Log($"> {_name}", file: file, line: line);

			Logger.IncreaseLevel();
		}

		public void Dispose()
		{
			Logger.DecreaseLevel();

			Logger.Log($"< {_name}", file: _file, line: _line);
		}
	}
}
