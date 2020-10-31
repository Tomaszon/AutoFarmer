using AutoFarmer.Models.Common;
using AutoFarmer.Properties;
using System;
using System.IO;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarmer.Services.InputHandling
{
	public static class NotificationPlayer
	{
		private static async void Play(Stream sound, int count)
		{
			await Task.Run(() =>
			{
				var player = new SoundPlayer(sound);

				for (int i = 0; i < count; i++)
				{
					player.PlaySync();
				}
			});
		}

		public static void Play(string message, NotificationType type, int count = 1)
		{
			if (type == NotificationType.None) return;

			switch (type)
			{
				case NotificationType.Click:
				{
					Play(Resources.click, count);
				}
				break;

				case NotificationType.ClickSingle:
				{
					Play(Resources.clickSingle, count);
				}
				break;

				case NotificationType.Error:
				{
					Play(Resources.error, count);
				}
				break;

				case NotificationType.Info:
				{
					Play(Resources.information, count);
				}
				break;

				case NotificationType.Voice:
				{
					Speak(message, count);
				}
				break;
			}
		}

		public static async void Speak(string textToSpeech, int count = 1)
		{
			await Task.Run(() =>
			{
				var file = Path.Combine(Directory.GetCurrentDirectory(), "textToSpeechCommandTmp.ps1");

				try
				{
					var command = $@"Add-Type -AssemblyName System.speech;
						$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;
						for ($i=0; $i -lt {count}; $i++)
						{{
							$speak.Speak(""{textToSpeech}"");
						}}";

					using (var sw = new StreamWriter(file, false, Encoding.UTF8))
					{
						sw.Write(command);
					}

					var start = new System.Diagnostics.ProcessStartInfo()
					{
						FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe",
						LoadUserProfile = false,
						UseShellExecute = false,
						CreateNoWindow = true,
						Arguments = $"-executionpolicy bypass -File {file}",
						WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
					};

					var p = System.Diagnostics.Process.Start(start);

					p.WaitForExit();
				}
				catch
				{
					// :'(
				}
				finally
				{
					File.Delete(file);
				}
			});
		}
	}
}
