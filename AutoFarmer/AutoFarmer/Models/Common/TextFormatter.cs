using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoFarmer.Models.Common
{
	public class TextFormatter
	{
		public int Level { get; set; }

		public int LineMaxLength { get; set; } = 50;

		public int TabSize { get; set; } = 4;

		private string Tab
		{
			get
			{
				string s = "";

				for (int i = 0; i < TabSize; i++)
				{
					s += " ";
				}

				return s;
			}
		}

		private string LevelTab
		{
			get
			{
				var s = "";

				for (int i = 0; i < Level; i++)
				{
					s += Tab;
				}

				return s;
			}
		}

		public string FormatMessage(string message, string file, int line)
		{
			message = ReplaceTabs(message);

			message = $"{LevelTab}{Path.GetFileName(file)} - {line}:\n{LevelTab}{message}";

			message = SmartBreak(message) + "\n\n";

			return message;
		}

		private string ReplaceTabs(string message)
		{
			return message.Replace("\t", Tab);
		}

		private string SmartBreak(string message)
		{
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
					if (lastLineLength + 1 > LineMaxLength)
					{
						if (buffer.Length < LineMaxLength - LevelTab.Length)
						{
							result.Append("\n" + LevelTab + buffer.ToString() + message[i]);
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

			message = result.ToString().TrimEnd();

			return message;

		}

		private string HardBreak(string message, int lastLineLength)
		{
			string result = "";

			var blockLength = LineMaxLength - LevelTab.Length;

			var l = new List<string>
			{
				message.Substring(0, LineMaxLength-lastLineLength)
			};

			for (int i = LineMaxLength - lastLineLength; i <= message.Length; i += blockLength)
			{
				l.Add(message.Substring(i, i + blockLength > message.Length ? message.Length - i : blockLength));
			}

			foreach (var block in l)
			{
				if (!string.IsNullOrWhiteSpace(block))
				{
					result += $"\n{LevelTab}{block}";
				}
			}

			return result.TrimStart('\n', ' ');
		}
	}
}
