using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoFarmer.Services.Logging
{
	public class LogFormatter : LogFormatterBase
	{
		public int Level { get; set; }

		public int InitialMaximumLineWidth { get; set; }

		private string Indent
		{
			get
			{
				string s = "";

				for (int i = 0; i < IndentSize; i++)
				{
					s += IndentCharacter;
				}

				return s;
			}
		}

		private string LevelIndent
		{
			get
			{
				var s = "";

				for (int i = 0; i < Level; i++)
				{
					s += Indent;
				}

				return s;
			}
		}

		public string FormatMessage(string message, string file, int line)
		{
			message = ReplaceTabs(message);

			var info = $"{LevelIndent}{Path.GetFileName(file)} - {line}:\n";

			message = IsMethodEnterOrExitMessage(message) ? FormatEnterAndExitMessage(info, message) : info + $"{LevelIndent}{message}";

			message = SmartBreak(message);

			return message + "\n\n";
		}

		private string FormatEnterAndExitMessage(string info, string message)
		{
			return info + (LevelIndent.Length >= 2 ? LevelIndent + message.Insert(2, Indent.Substring(2)) : message.Insert(2, Indent.Substring(2)));
		}

		private bool IsMethodEnterOrExitMessage(string message)
		{
			return message.StartsWith("->") || message.StartsWith("<-");
		}

		private string ReplaceTabs(string message)
		{
			return message.Replace("\t", Indent);
		}

		private string SmartBreak(string message)
		{
			HandleDynamicMaximumLineLength();

			message = $"{message.TrimEnd()} ";

			StringBuilder result = new StringBuilder();

			StringBuilder buffer = new StringBuilder();

			for (int i = 0; i < message.Length; i++)
			{
				int lastLineLength = result.Length - result.ToString().LastIndexOf('\n') - 1 + buffer.Length;

				if (Regex.IsMatch(message[i].ToString(), "\\w"))
				{
					buffer.Append(message[i]);

					if (i == message.Length - 1)
					{
						result.Append(buffer.ToString());
					}
				}
				else
				{
					if (lastLineLength + 1 > MaximumLineLength)
					{
						if (buffer.Length < MaximumLineLength - LevelIndent.Length)
						{
							result.Append("\n" + LevelIndent + buffer.ToString() + message[i]);
						}
						else
						{
							result.Append(HardBreak(buffer.ToString() + message[i], lastLineLength - buffer.Length));
						}
					}
					else
					{
						result.Append(buffer.ToString() + message[i]);
					}

					buffer.Clear();
				}
			}

			message = result.ToString();

			message = RemoveReduntandLineBreaks(message);

			return message.TrimEnd().TrimEnd(IndentCharacter);
		}

		private void HandleDynamicMaximumLineLength()
		{
			MaximumLineLength = Math.Max(InitialMaximumLineWidth, MinimumLineLength + LevelIndent.Length);
		}

		private string RemoveReduntandLineBreaks(string message)
		{
			while (message.Contains($"\n{LevelIndent}\n{LevelIndent}"))
			{
				message = message.Replace($"\n{LevelIndent}\n{LevelIndent}", $"\n{LevelIndent}");
			}

			return message;
		}

		private string HardBreak(string message, int lastLineLength)
		{
			string result = "";

			var blockLength = MaximumLineLength - LevelIndent.Length;

			var firstBlockLength = MaximumLineLength - lastLineLength;

			var blocks = new List<string>()
			{
				message.Substring(0, firstBlockLength)
			};

			for (int i = firstBlockLength; i <= message.Length; i += blockLength)
			{
				blocks.Add(message.Substring(i, i + blockLength > message.Length ? message.Length - i : blockLength));
			}

			result += blocks[0];
			for (int i = 1; i < blocks.Count; i++)
			{
				result += $"\n{LevelIndent}{blocks[i]}";
			}

			return result;
		}
	}
}
