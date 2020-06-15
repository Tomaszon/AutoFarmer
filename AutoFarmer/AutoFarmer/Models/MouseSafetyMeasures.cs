using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace AutoFarmer.Models
{
	public class MouseSafetyMeasures
	{
		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out Point lpPoint);

		public bool IsEnabled { get; set; } = true;

		public SerializablePoint MouseSafePosition { get; set; }

		public int SafeAreaRadius { get; set; }

		public int ActionPositionToleranceRadius { get; set; }

		public static MouseSafetyMeasures Instance { get; private set; }

		public static void FromConfig()
		{
			Instance = JsonConvert.DeserializeObject<MouseSafetyMeasures>(File.ReadAllText(Config.Instance.MouseSafetyMeasuresConfigPath));
		}

		private Point GetCursorCurrentPosition()
		{
			GetCursorPos(out Point currentPosition);

			return currentPosition;
		}

		public bool IsMouseInActionPosition(Point actionPosition)
		{
			return IsInRange(actionPosition, ActionPositionToleranceRadius);
		}

		public bool IsMouseInSafePosition()
		{
			return IsInRange(MouseSafePosition, SafeAreaRadius);
		}

		private bool IsInRange(Point center, int maxDistance)
		{
			var currentPosition = GetCursorCurrentPosition();

			Size difference = (SerializablePoint)center - currentPosition;

			double distance = Math.Sqrt(Math.Pow(difference.Width, 2) + Math.Pow(difference.Height, 2));

			return distance <= maxDistance;
		}

		public void CheckForIntentionalEmergencyStop()
		{
			if (IsEnabled && !IsMouseInSafePosition())
			{
				throw new AutoFarmerException("Intentional emergency stop!");
			}
		}

		public void CheckForIntentionalEmergencyStop(Point actionPosition)
		{
			if (IsEnabled && !IsMouseInActionPosition(actionPosition))
			{
				throw new AutoFarmerException("Intentional emergency stop!");
			}
		}
	}
}