using AutoFarmer.Models.Common;
using AutoFarmer.Properties;
using AutoFarmer.Services.Logging;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace AutoFarmer.Services.InputHandling
{
	public static class NotificationPlayer
	{
		private static async void Play(Stream sound, int count)
		{
			using var log = Logger.LogBlock();

			await Task.Run(() =>
			{
				using var log = Logger.LogBlock();

				var player = new SoundPlayer(sound);

				for (int i = 0; i < count; i++)
				{
					player.PlaySync();
				}
			});
		}

		public static void Play(NotificationType type, int count = 1)
		{
			using var log = Logger.LogBlock();

			if (type == NotificationType.None) return;

			switch (type)
			{
				case NotificationType.Click:
				{
					Play(Resources.soundnavigate, count);
				}
				break;

				case NotificationType.Error:
				{
					Play(Resources.sounderror, count);
				}
				break;

				case NotificationType.Info:
				{
					Play(Resources.soundexclamation, count);
				}
				break;
			}
		}
	}
}
