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

		public static void GraphicalLog(Bitmap source, Point[] clickPoints, Rectangle[] searchRectangles, string templateName, string searchRectangleName, List<SerializableRectangle> searchAreas)
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
			searchAreas.ForEach(a =>
				g.DrawRectangle(new Pen(new SolidBrush(Color.Red), searchRectangles.Any(r => a.Contains(r)) ? 3 : 1), a));

			searchAreas.ForEach(a =>
				searchAreas.Where(b =>
					a.IntersectsWith(b) && a.Location != b.Location && a.Size != b.Size).ToList().ForEach(r =>
						g.DrawRectangle(new Pen(new SolidBrush(Color.Purple), 1), r)));
		}

		private static void HighlightFind(Graphics g, Rectangle rectangle, Point clickPoint)
		{
			g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 3), rectangle);

			g.DrawRectangle(Pens.Red, new Rectangle(clickPoint.X - 1, clickPoint.Y - 1, 3, 3));
		}
	}
}
