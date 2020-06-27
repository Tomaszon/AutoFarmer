using AutoFarmer.Models.Common;
using AutoFarmer.Properties;
using System.Media;

namespace AutoFarmer.Models.InputHandling
{
	public static class NotificationPlayer
	{
		private static void PlayError()
		{
			new SoundPlayer(Resources.sounderror).PlaySync();
		}

		private static void PlayClick()
		{
			new SoundPlayer(Resources.soundnavigate).PlaySync();
		}

		private static void PlayInfo()
		{
			new SoundPlayer(Resources.soundexclamation).PlaySync();
		}

		public static void Play(NotificationType type, int count = 1)
		{
			if (type == NotificationType.None) return;

			for (int i = 0; i < count; i++)
			{
				switch (type)
				{
					case NotificationType.Click:
						PlayClick();
						break;
					case NotificationType.Error:
						PlayError();
						break;
					case NotificationType.Info:
						PlayInfo();
						break;
				}
			}
		}
	}
}
