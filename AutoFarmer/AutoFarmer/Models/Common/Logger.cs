using AutoFarmer.Models.ImageMatching;
using AutoFarmer.Models.InputHandling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models
{
	public static class Logger
	{
		private static Guid _guid = Guid.NewGuid();

		public static void Log(string message = "", NotificationType notificationType = NotificationType.None, int count = 1)
		{
			try
			{
				Console.WriteLine(message);
				Console.WriteLine();

				if (Config.Instance.FileLogging)
				{
					Directory.CreateDirectory(Config.Instance.LogDirectory);

					File.AppendAllText(Path.Combine(Config.Instance.LogDirectory, $"{_guid}.log"), message + "\n\n");
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
			try
			{
				if (Config.Instance.GraphicalLogging)
				{
					using (Graphics g = Graphics.FromImage(matchCollection.Source))
					{
						Directory.CreateDirectory(Path.Combine(Config.Instance.LogDirectory, _guid.ToString()));

						matchCollection.Matches.ForEach(m => HighlightFind(g, m.MatchRectangle, m.ClickPoint));

						HighlightSearchAreas(g, matchCollection.SearchAreas, matchCollection.Matches.Select(m => m.MatchRectangle));

						var fileName = Path.Combine(Config.Instance.LogDirectory, _guid.ToString(), $"{templateName}-{searchRectangleName}");

						matchCollection.Source.Save($"{fileName}.png");
						matchCollection.SearchImage.Save($"{fileName}-SearchImage.png");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected exception occured durning logging:\n " + ex.ToString());
			}
		}

		private static void HighlightSearchAreas(Graphics g, List<SerializableRectangle> searchAreas, IEnumerable<SerializableRectangle> searchRectangles)
		{
			foreach (var area1 in searchAreas)
			{
				foreach (var area2 in searchAreas)
				{
					if (((Rectangle)area1).IntersectsWith((Rectangle)area2) && area1 != area2)
					{
						Rectangle r = new Rectangle(area1.X, area1.Y, area1.W, area1.H);
						r.Intersect((Rectangle)area2);

						g.FillRectangle(new SolidBrush(Color.FromArgb(16, 128, 255, 128)), r);
					}
				}
			}

			searchAreas.Where(a =>
				searchRectangles.All(r =>
					!((Rectangle)a).Contains((Rectangle)r))).ToList().ForEach(e =>
					{
						var rectangle = new Rectangle(e.X, e.Y, e.W - 1, e.H - 1);
						g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), rectangle);
						g.DrawRectangle(new Pen(new SolidBrush(Color.Yellow), 1), rectangle);
					});

			searchAreas.Where(a =>
				searchRectangles.All(r =>
					((Rectangle)a).Contains((Rectangle)r))).ToList().ForEach(e =>
						g.DrawRectangle(new Pen(new SolidBrush(Color.Orange), 3), new Rectangle(e.X, e.Y, e.W - 1, e.H - 1)));
		}

		private static void HighlightFind(Graphics g, SerializableRectangle rectangle, SerializablePoint clickPoint)
		{
			g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 3), new Rectangle(rectangle.X, rectangle.Y, rectangle.W - 1, rectangle.H - 1));

			g.DrawRectangle(Pens.Red, new Rectangle(clickPoint.X - 1, clickPoint.Y - 1, 2, 2));
		}
	}
}
