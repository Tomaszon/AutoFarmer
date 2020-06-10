using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarmer
{
	public static class Logger
	{
		private static Guid _guid = Guid.NewGuid();

		public static void Log(string message = "", NotificationType notificationType = NotificationType.None, int count = 1)
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

		public static void GraphicalLog(Bitmap source, Point clickPoint, SearchRectangle searchRectangle, string templateName, string searchRectangleName)
		{
			if (Config.Instance.GraphicalLogging)
			{
				Directory.CreateDirectory(Path.Combine(Config.Instance.LogDirectory, _guid.ToString()));

				HighlightFind(source, searchRectangle, clickPoint);

				source.Save(Path.Combine(Config.Instance.LogDirectory, _guid.ToString(), $"{templateName}-{searchRectangleName}.png"));
			}
		}

		private static void HighlightFind(Bitmap bitmap, Rectangle rectangle, Point clickPoint)
		{
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.DrawRectangle(Pens.Red, rectangle);

				g.DrawRectangle(Pens.Red, new Rectangle(clickPoint.X - 1, clickPoint.Y - 1, 3, 3));
			}
		}
	}
}
