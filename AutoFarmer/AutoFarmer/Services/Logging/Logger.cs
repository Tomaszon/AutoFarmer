using AutoFarmer.Models.Common;
using AutoFarmer.Models.ImageMatching;
using AutoFarmer.Services.InputHandling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AutoFarmer.Services.Logging
{
	public class Logger : LoggerBase
	{
		public static Logger Instance { get; set; }

		public string SessionId { get; private set; }

		public LogFormatter Formatter { get; set; } = new LogFormatter();

		internal static void RefreshSessionId()
		{
			Instance.SessionId = DateTime.Now.ToString("yyyyMMdd-hhmmss");
		}

		public static void FromConfig()
		{
			Instance = LoggerOptions.FromJsonFile(Path.Combine(Config.Instance.ConfigDirectory, "loggerConfig.json"));

			RefreshSessionId();
		}

		public static void IncreaseLevel()
		{
			Instance.Formatter.Level++;
		}

		public static void DecreaseLevel()
		{
			Instance.Formatter.Level--;
		}

		public static LogBlock LogBlock(string name = default, [CallerFilePath] string file = default, [CallerMemberName] string method = default, [CallerLineNumber] int line = default)
		{
			return new LogBlock(name, file, method, line);
		}

		public static void Log(string message, NotificationType notificationType = NotificationType.None, int count = 1, [CallerFilePath] string file = default, [CallerLineNumber] int line = default, bool fileLog = true)
		{
			try
			{
				message = Instance.Formatter.FormatMessage(message, file, line);

				Console.Write(message);

				if (Instance.FileLogging && fileLog)
				{
					Directory.CreateDirectory(Instance.LogDirectory);

					File.AppendAllText(Path.Combine(Instance.LogDirectory, $"{Instance.SessionId}.log"), message);
				}

				NotificationPlayer.Play(notificationType, count);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected exception occured durning logging:\n " + ex.ToString());
			}
		}

		public static void GraphicalLog(ImageMatchResult matchCollection, string templateName, string searchRectangleName)
		{
			using var log = LogBlock();

			try
			{
				if (Instance.GraphicalLogging)
				{
					using Graphics g = Graphics.FromImage(matchCollection.Source);

					Directory.CreateDirectory(Path.Combine(Instance.LogDirectory, Instance.SessionId));

					matchCollection.Matches.ForEach(m => HighlightFind(g, m.MatchRectangle, m.ClickPoint));

					HighlightSearchAreas(g, matchCollection.SearchAreas, matchCollection.Matches.Select(m => m.MatchRectangle));

					var fileName = Path.Combine(Instance.LogDirectory, Instance.SessionId, $"{templateName}-{searchRectangleName}");

					matchCollection.Source.Save($"{fileName}.png");
					matchCollection.SearchImage.Save($"{fileName}-SearchImage.png");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected exception occured durning logging:\n " + ex.ToString());
			}
		}

		private static void HighlightSearchAreas(Graphics g, List<SerializableRectangle> searchAreas, IEnumerable<SerializableRectangle> searchRectangles)
		{
			using var log = LogBlock();

			foreach (var area1 in searchAreas)
			{
				foreach (var area2 in searchAreas)
				{
					if (((Rectangle)area1).IntersectsWith((Rectangle)area2) && area1 != area2)
					{
						Rectangle r = new Rectangle(area1.Position.X, area1.Position.Y, area1.Size.W, area1.Size.H);
						r.Intersect((Rectangle)area2);

						g.FillRectangle(new SolidBrush(Color.FromArgb(16, 128, 255, 128)), r);
					}
				}
			}

			searchAreas.Where(a =>
				searchRectangles.All(r =>
					!((Rectangle)a).Contains((Rectangle)r))).ToList().ForEach(e =>
					{
						var rectangle = new Rectangle(e.Position.X, e.Position.Y, e.Size.W - 1, e.Size.H - 1);
						g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), rectangle);
						g.DrawRectangle(new Pen(new SolidBrush(Color.Yellow), 1), rectangle);
					});

			searchAreas.Where(a =>
				searchRectangles.All(r =>
					((Rectangle)a).Contains((Rectangle)r))).ToList().ForEach(e =>
						g.DrawRectangle(new Pen(new SolidBrush(Color.Orange), 3), new Rectangle(e.Position.X, e.Position.Y, e.Size.W - 1, e.Size.H - 1)));
		}

		private static void HighlightFind(Graphics g, SerializableRectangle rectangle, SerializablePoint clickPoint)
		{
			using var log = Logger.LogBlock();

			g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 3), new Rectangle(rectangle.Position.X, rectangle.Position.Y, rectangle.Size.W - 1, rectangle.Size.H - 1));

			g.DrawRectangle(Pens.Red, new Rectangle(clickPoint.X - 1, clickPoint.Y - 1, 2, 2));
		}
	}
}
