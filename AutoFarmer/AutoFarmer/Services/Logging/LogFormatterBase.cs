namespace AutoFarmer.Services.Logging
{
	public abstract class LogFormatterBase
	{
		public int IndentSize { get; set; } = 4;

		public int MaximumLineLength { get; set; } = 150;

		public int MinimumLineLength { get; set; } = 10;

		public char IndentCharacter { get; set; } = ' ';
	}
}
