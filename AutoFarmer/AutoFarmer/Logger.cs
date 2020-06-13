using System;
using System.Drawing;
using System.IO;

namespace AutoFarmer
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

		public static void GraphicalLog(Bitmap source, Point[] clickPoint, Rectangle[] searchRectangle, string templateName, string searchRectangleName)
		{
			try
			{
				if (Config.Instance.GraphicalLogging)
				{
					Directory.CreateDirectory(Path.Combine(Config.Instance.LogDirectory, _guid.ToString()));

					for (int i = 0; i < searchRectangle.Length; i++)
					{
						HighlightFind(source, searchRectangle[i], clickPoint[i]);
					}

					source.Save(Path.Combine(Config.Instance.LogDirectory, _guid.ToString(), $"{templateName}-{searchRectangleName}.png"));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected exception occured durning logging:\n " + ex.ToString());
			}
		}

		private static void HighlightFind(Bitmap bitmap, Rectangle rectangle, Point clickPoint)
		{
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 3), rectangle);

				g.DrawRectangle(Pens.Red, new Rectangle(clickPoint.X - 1, clickPoint.Y - 1, 3, 3));
			}
		}
	}
}
