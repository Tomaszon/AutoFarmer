using AutoFarmer.Models.Common;
using AutoFarmer.Services.Logging;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace AutoFarmer.Services
{
	public class MouseSafetyMeasures
	{
		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out Point lpPoint);

		public bool IsEnabled { get; set; } = true;

		public SerializablePoint LastActionPosition { get; set; }

		public double SafeAreaRadius { get; set; }

		public static MouseSafetyMeasures Instance { get; private set; }

		public static void FromConfig()
		{
			using var log = Logger.LogBlock();

			Instance = JsonConvert.DeserializeObject<MouseSafetyMeasures>(File.ReadAllText(Config.Instance.MouseSafetyMeasuresConfigPath));
		}

		public static SerializablePoint GetCursorCurrentPosition()
		{
			using var log = Logger.LogBlock();

			GetCursorPos(out Point currentPosition);

			return (SerializablePoint)currentPosition;
		}

		public static bool IsMouseInSafePosition()
		{
			using var log = Logger.LogBlock();

			var currentPosition = GetCursorCurrentPosition();

			SerializableSize difference = Instance.LastActionPosition - currentPosition;

			double distance = Math.Round(Math.Sqrt(Math.Pow(difference.W, 2) + Math.Pow(difference.H, 2)), 2);

			Logger.Log($"Safe position check: current mouse position: {currentPosition}, last action position: {Instance.LastActionPosition}, distance: {distance}");

			return distance <= Instance.SafeAreaRadius;
		}

		public static void CheckForIntentionalEmergencyStop()
		{
			if (Instance.IsEnabled && !IsMouseInSafePosition())
			{
				using var log = Logger.LogBlock();

				throw new AutoFarmerException("Intentional emergency stop!");
			}
		}
	}
}