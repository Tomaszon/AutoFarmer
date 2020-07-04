using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace AutoFarmer.Models.Common
{
	public class WrapText
	{
		public static List<string> Wrap(string text, double pixels, string fontFamily, float emSize)
		{
			string[] originalLines = text.Split(new string[] { " " },
				StringSplitOptions.None);

			List<string> wrappedLines = new List<string>();

			StringBuilder actualLine = new StringBuilder();
			double actualWidth = 0;

			foreach (var item in originalLines)
			{
				FormattedText formatted = new FormattedText(item,
					CultureInfo.CurrentCulture,
					System.Windows.FlowDirection.LeftToRight,
					new Typeface(fontFamily), emSize, Brushes.Black);

				actualLine.Append(item + " ");
				actualWidth += formatted.Width;

				if (actualWidth > pixels)
				{
					wrappedLines.Add(actualLine.ToString());
					actualLine.Clear();
					actualWidth = 0;
				}
			}

			if (actualLine.Length > 0)
				wrappedLines.Add(actualLine.ToString());

			return wrappedLines;
		}

		public static string Split(string text, int partLength)
		{
			var partCount = Math.Ceiling((double)text.Length / partLength);

			var result = "";

			for (int i = 0; i < partCount; i++)
			{
				var index = i * partLength;
				var lengthLeft = Math.Min(partLength, text.Length - index);
				var line = text.Substring(index, lengthLeft);

				result += line + "\n";
			}

			return result;
		}

		public static string Split2(string text, int maxLineLength)
		{
			var list = new List<string>();

			int currentIndex;
			var lastWrap = 0;
			var whitespace = new[] { ' ', '\r', '\n', '\t' };
			do
			{
				currentIndex = lastWrap + maxLineLength > text.Length ? text.Length : (text.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' }, Math.Min(text.Length - 1, lastWrap + maxLineLength)) + 1);
				if (currentIndex <= lastWrap)
					currentIndex = Math.Min(lastWrap + maxLineLength, text.Length);
				list.Add(text.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
				lastWrap = currentIndex;
			}
			while (currentIndex < text.Length);

			return string.Concat(list.Select(e => e + "\n"));
		}

		/// <summary>
		/// Word wraps the given text to fit within the specified width.
		/// </summary>
		/// <param name="text">Text to be word wrapped</param>
		/// <param name="width">Width, in characters, to which the text
		/// should be word wrapped</param>
		/// <returns>The modified text</returns>
		public static string Split3(string text, int width)
		{
			int pos, next;
			StringBuilder sb = new StringBuilder();

			// Parse each line of text
			for (pos = 0; pos < text.Length; pos = next)
			{
				// Find end of line
				int eol = text.IndexOf(Environment.NewLine, pos);
				if (eol == -1)
					next = eol = text.Length;
				else
					next = eol + Environment.NewLine.Length;

				// Copy this line of text, breaking into smaller lines as needed
				if (eol > pos)
				{
					do
					{
						int len = eol - pos;
						if (len > width)
							len = BreakLine(text, pos, width);
						sb.Append(text, pos, len);
						sb.Append(Environment.NewLine);

						// Trim whitespace following break
						pos += len;
						while (pos < eol && char.IsWhiteSpace(text[pos]))
							pos++;
					} while (eol > pos);
				}
				else
				{
					sb.Append(Environment.NewLine); // Empty line
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Locates position to break the given line so as to avoid
		/// breaking words.
		/// </summary>
		/// <param name="text">String that contains line of text</param>
		/// <param name="pos">Index where line of text starts</param>
		/// <param name="max">Maximum line length</param>
		/// <returns>The modified line length</returns>
		private static int BreakLine(string text, int pos, int max)
		{
			// Find last whitespace in line
			int i = max;
			while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
				i--;

			// If no whitespace found, break at maximum length
			if (i < 0)
				return max;

			// Find start of whitespace
			while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
				i--;

			// Return length of text before whitespace
			return i + 1;
		}
	}
}
