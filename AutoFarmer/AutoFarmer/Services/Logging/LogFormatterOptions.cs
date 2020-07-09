using System;
using System.Text.RegularExpressions;

namespace AutoFarmer.Services.Logging
{
	public class LogFormatterOptions : LogFormatterBase
	{
		public LogFormatter ToLogFormatter()
		{
			return new LogFormatter()
			{
				IndentSize = Math.Max(IndentSize, 2),
				MaximumLineLength = Math.Max(MaximumLineLength, MinimumLineLength),
				InitialMaximumLineWidth = Math.Max(MaximumLineLength, MinimumLineLength),
				IndentCharacter = Regex.IsMatch(IndentCharacter.ToString(), "\\w|\\s") ? ' ' : IndentCharacter,
				MinimumLineLength = Math.Min(MinimumLineLength, MaximumLineLength)
			};
		}
	}
}
