using AForge;
using AutoFarmer.Models.Common;
using AutoFarmer.Services.Logging;
using System;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace AutoFarmer.Services.InputHandling
{
	public class InputSimulator
	{
		public int Delay { get; set; } = 250;

		public SerializableSize ScreenSize { get; set; }

		public static InputSimulator Instance { get; set; }

		public static void FromConfig()
		{
			Instance = new InputSimulator
			{
				ScreenSize = Config.Instance.ScreenSize
			};
		}

		public static void Simulate(string[] inputActionNames, SerializablePoint actionPosition, int additionalDelay = 0)
		{
			using var log = Logger.LogBlock();

			if (inputActionNames is null) return;

			MouseSafetyMeasures.Instance.LastActionPosition = actionPosition;

			foreach (var action in inputActionNames)
			{
				MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

				if (TryParse(action, out SerializablePoint p))
				{
					MoveMouseTo(p);
				}
				else if (TryParse(action, out var stepSize, out var scanSize, out var delay))
				{
					ScanScreenForFadedUI(stepSize, scanSize, delay, additionalDelay);
				}
				else if (TryParse(action, out int l))
				{
					HoldEvent(l);
				}
				else if (Enum.TryParse<MouseAction>(action, true, out var mouseAction))
				{
					ClickEvent(mouseAction, additionalDelay);
				}
				else if (Enum.TryParse<VirtualKeyCode>(action, true, out var keyboardAction))
				{
					KeyboardEvent(keyboardAction, additionalDelay);
				}
				else
				{
					throw new AutoFarmerException($"Unknown input action: {action}");
				}
			}
		}

		private static bool TryParse(string value, out int length)
		{
			using var log = Logger.LogBlock();

			length = 0;

			Regex regex = new Regex($"{MouseAction.LeftHold}:(?<l>(\\d)+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var match = regex.Match(value);

			if (match.Success && int.TryParse(match.Groups["l"].Value, out var l))
			{
				length = l;

				return true;
			}

			return false;
		}

		private static bool TryParse(string value, out int stepSize, out int scanSize, out int delay)
		{
			using var log = Logger.LogBlock();

			stepSize = 0;
			scanSize = 0;
			delay = 0;

			Regex regex = new Regex($"{MouseAction.ScanScreenForFadedUI}:(?<w>(\\d)+),(?<h>(\\d)+),(?<d>(\\d)+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var match = regex.Match(value);

			if (match.Success && int.TryParse(match.Groups["w"].Value, out stepSize) && int.TryParse(match.Groups["h"].Value, out scanSize) && int.TryParse(match.Groups["d"].Value, out delay))
			{
				return true;
			}

			return false;
		}

		private static bool TryParse(string value, out SerializablePoint point)
		{
			using var log = Logger.LogBlock();

			point = null;

			Regex regex = new Regex($"{MouseAction.Move}:(?<x>(\\d)+),(?<y>(\\d)+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var match = regex.Match(value);

			if (match.Success && int.TryParse(match.Groups["x"].Value, out var x) && int.TryParse(match.Groups["y"].Value, out var y))
			{
				point = new SerializablePoint { X = x, Y = y };

				return true;
			}

			return false;
		}

		private static void KeyboardEvent(VirtualKeyCode virtualKeyCode, int additionalDelay = 0)
		{
			using var log = Logger.LogBlock();

			new WindowsInput.InputSimulator().Keyboard.KeyPress(virtualKeyCode);

			Logger.Log($"Virtual key pressed: {virtualKeyCode}", NotificationType.ClickSingle);

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		private static void ClickEvent(MouseAction mouseAction, int additionalDelay = 0)
		{
			using var log = Logger.LogBlock();

			var simulator = new WindowsInput.InputSimulator();

			switch (mouseAction)
			{
				case MouseAction.LeftClick:
				{
					Logger.Log("Left mouse click", NotificationType.ClickSingle, 2);

					simulator.Mouse.LeftButtonClick();

					break;
				}
				case MouseAction.LeftDoubleClick:
				{
					Logger.Log("Left mouse double click", NotificationType.ClickSingle, 4);//TODO use unique sound

					simulator.Mouse.LeftButtonDoubleClick();

					break;
				}
			}

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		private static void HoldEvent(int length)
		{
			using var log = Logger.LogBlock();

			var simulator = new WindowsInput.InputSimulator();

			Logger.Log("Left mouse hold", NotificationType.ClickSingle);

			simulator.Mouse.LeftButtonDown();

			Thread.Sleep(length);

			Logger.Log("Left mouse release", NotificationType.ClickSingle);

			simulator.Mouse.LeftButtonUp();
		}

		/// <summary>
		/// Moves the cursor to the given position.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="additionalDelay"></param>
		public static void MoveMouseTo(SerializablePoint point, int additionalDelay = 0)
		{
			using var log = Logger.LogBlock();

			Logger.Log($"Mouse move to: {point}");

			SmoothMouseMove(point);

			MouseSafetyMeasures.Instance.LastActionPosition = MouseSafetyMeasures.GetCursorCurrentPosition();

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		public static void ScanScreenForFadedUI(int stepSize, int scanSize, int delay, int additionalDelay = 0)
		{
			using var log = Logger.LogBlock();

			Logger.Log("Scanning for faded UI");

			NormalizedMouseMove(new SerializablePoint());

			for (int y = 0; y <= Instance.ScreenSize.H; y += scanSize)
			{
				SmoothMouseMove(new SerializablePoint() { X = Instance.ScreenSize.W, Y = y }, stepSize, delay);

				SmoothMouseMove(new SerializablePoint() { X = 0, Y = y }, stepSize, delay);

				if (y == Instance.ScreenSize.H) break;

				y = y + scanSize > Instance.ScreenSize.H ? Instance.ScreenSize.H - scanSize : y;
			}

			NormalizedMouseMove(MouseSafetyMeasures.Instance.LastActionPosition);

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		private static void SmoothMouseMove(SerializablePoint to, int stepSize = 60, int delay = 10)
		{
			var from = MouseSafetyMeasures.GetCursorCurrentPosition();

			var difference = to - from;

			var distance = SerializablePoint.Distance(from, to);

			var step = (int)(distance / stepSize);

			for (int i = 0; i < step; i++)
			{
				NormalizedMouseMove(from + new SerializableSize() { W = (int)((double)difference.W / step * i), H = (int)((double)difference.H / step * i) });

				Thread.Sleep(delay);
			}

			NormalizedMouseMove(to);
		}

		private static void NormalizedMouseMove(SerializablePoint point)
		{
			new WindowsInput.InputSimulator().Mouse.MoveMouseTo(point.X * (65536.0 / Instance.ScreenSize.W) + 1, point.Y * (65536.0 / Instance.ScreenSize.H) + 1);
		}
	}
}
