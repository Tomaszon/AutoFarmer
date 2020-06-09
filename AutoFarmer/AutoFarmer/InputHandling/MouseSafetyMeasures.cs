using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AutoFarmer
{
	public class MouseSafetyMeasures
	{
		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out Point lpPoint);

		public SerializablePoint MouseSafePosition { get; set; }

		public int SafeAreaRadius { get; set; }

		public bool IsMouseInSafePosition()
		{
			GetCursorPos(out Point currentPosition);

			Size difference = MouseSafePosition - currentPosition;

			double distance = Math.Sqrt(Math.Pow(difference.Width, 2) + Math.Pow(difference.Height, 2));

			return distance <= SafeAreaRadius;
		}
	}
}