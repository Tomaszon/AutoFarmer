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

		public static void GraphicalLog(Bitmap source, Point[] clickPoints, Rectangle[] searchRectangles, string templateName, string searchRectangleName, List<Rectangle> searchAreas)
		{
			try
			{
				if (Config.Instance.GraphicalLogging)
				{
					using (Graphics g = Graphics.FromImage(source))
					{
						Directory.CreateDirectory(Path.Combine(Config.Instance.LogDirectory, _guid.ToString()));

						for (int i = 0; i < searchRectangles.Length; i++)
						{
							HighlightFind(g, searchRectangles[i], clickPoints[i]);
						}

						HighlightSearchAreas(g, searchAreas.Select(r => (Rectangle)r).ToList(), searchRectangles);

						source.Save(Path.Combine(Config.Instance.LogDirectory, _guid.ToString(), $"{templateName}-{searchRectangleName}.png"));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected exception occured durning logging:\n " + ex.ToString());
			}
		}

		private static void HighlightSearchAreas(Graphics g, List<Rectangle> searchAreas, Rectangle[] searchRectangles)
		{
			foreach (var area1 in searchAreas)
			{
				foreach (var area2 in searchAreas)
				{
					if (area1.IntersectsWith(area2) && area1 != area2)
					{
						Rectangle r = new Rectangle(area1.Location, area1.Size);
						r.Intersect(area2);

						g.FillRectangle(new SolidBrush(Color.FromArgb(16, 128, 255, 128)), r);
					}
				}
			}
			
			searchAreas.Where(a => searchRectangles.All(r => !a.Contains(r))).ToList().ForEach(e =>
				g.DrawRectangle(new Pen(new SolidBrush(Color.Yellow), 1), new Rectangle(e.X, e.Y, e.Width - 1, e.Height - 1)));

			searchAreas.Where(a => searchRectangles.All(r => a.Contains(r))).ToList().ForEach(e =>
				g.DrawRectangle(new Pen(new SolidBrush(Color.Orange), 3), new Rectangle(e.X, e.Y, e.Width - 1, e.Height - 1)));

			Region region = new Region(new Rectangle(new Point(0, 0), Config.Instance.ScreenSize));

			searchAreas.Where(a => searchRectangles.All(r => a.Contains(r))).ToList().ForEach(a => region.Exclude(a));

			g.FillRegion(new SolidBrush(Color.FromArgb(64, 0, 0, 0)), region);
		}

		private static void HighlightFind(Graphics g, Rectangle rectangle, Point clickPoint)
		{
			g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 3), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1));

			g.DrawRectangle(Pens.Red, new Rectangle(clickPoint.X - 1, clickPoint.Y - 1, 2, 2));
		}
	}
}
