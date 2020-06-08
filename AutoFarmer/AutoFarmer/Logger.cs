using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarmer
{
	public static class Logger
	{
		public static void Log(string message = "", NotificationType notificationType = NotificationType.None, int count = 1)
		{
			Console.WriteLine(message);
			Console.WriteLine();

			NotificationPlayer.Play(notificationType, count);
		}
	}
}
