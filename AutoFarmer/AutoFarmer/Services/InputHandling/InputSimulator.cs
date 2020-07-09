using AutoFarmer.Models.Common;
using System;
using System.Text.RegularExpressions;
using System.Threading;
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
			if (inputActionNames is null) return;

			MouseSafetyMeasures.Instance.LastActionPosition = actionPosition;

			foreach (var action in inputActionNames)
			{
				MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

				if (TryParse(action, out SerializablePoint p))
				{
					MoveMouseTo(p);
				}
				else if (TryParse(action, out SerializableSize s, out var delay))
				{
					ScanScreenForFadedUI(s, delay, additionalDelay);
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

		private static bool TryParse(string value, out SerializableSize size, out int delay)
		{
			size = null;
			delay = 0;

			Regex regex = new Regex($"{MouseAction.ScanScreenForFadedUI}:(?<w>(\\d)+),(?<h>(\\d)+),(?<d>(\\d)+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var match = regex.Match(value);

			if (match.Success && int.TryParse(match.Groups["w"].Value, out var w) && int.TryParse(match.Groups["h"].Value, out var h) && int.TryParse(match.Groups["d"].Value, out var d))
			{
				size = new SerializableSize { W = w, H = h };
				delay = d;

				return true;
			}

			return false;
		}

		private static bool TryParse(string value, out SerializablePoint point)
		{
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
			new WindowsInput.InputSimulator().Keyboard.KeyPress(virtualKeyCode);

			Logger.Log($"Virtual key pressed: {virtualKeyCode}", NotificationType.Click);

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		private static void ClickEvent(MouseAction mouseAction, int additionalDelay = 0)
		{
			var simulator = new WindowsInput.InputSimulator();

			switch (mouseAction)
			{
				case MouseAction.LeftClick:
				{
					Logger.Log("Left mouse click", NotificationType.Click);

					simulator.Mouse.LeftButtonClick();

					break;
				}
				case MouseAction.LeftDoubleClick:
				{
					Logger.Log("Left mouse double click", NotificationType.Click, 1);//TODO use unique sound

					simulator.Mouse.LeftButtonDoubleClick();

					break;
				}
			}

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		private static void HoldEvent(int length)
		{
			var simulator = new WindowsInput.InputSimulator();

			Logger.Log("Left mouse hold", NotificationType.Click, 1);//TODO use unique sound

			simulator.Mouse.LeftButtonDown();

			Thread.Sleep(length);

			Logger.Log("Left mouse release", NotificationType.Click, 1);//TODO use unique sound

			simulator.Mouse.LeftButtonUp();
		}

		/// <summary>
		/// Moves the cursor to the given position.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="additionalDelay"></param>
		public static void MoveMouseTo(SerializablePoint point, int additionalDelay = 0, bool log = true)
		{
			if (log) Logger.Log($"Mouse move to: {point}");

			new WindowsInput.InputSimulator().Mouse.MoveMouseTo(point.X * (65536.0 / Instance.ScreenSize.W) + 1, point.Y * (65536.0 / Instance.ScreenSize.H) + 1);

			MouseSafetyMeasures.Instance.LastActionPosition = MouseSafetyMeasures.GetCursorCurrentPosition();

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		public static void ScanScreenForFadedUI(SerializableSize s, int delay = 0, int additionalDelay = 0)
		{
			Logger.Log("Scanning for faded UI");

			for (int y = 0; y <= Instance.ScreenSize.H; y += s.H)
			{
				for (int x = 0; x <= Instance.ScreenSize.W; x += s.W)
				{
					MoveMouseTo(new SerializablePoint() { X = x, Y = y }, delay - Instance.Delay, false);

					if (x == Instance.ScreenSize.W) break;

					x = x + s.W > Instance.ScreenSize.W ? Instance.ScreenSize.W - s.W : x;
				}

				if (y == Instance.ScreenSize.H) break;

				y = y + s.H > Instance.ScreenSize.H ? Instance.ScreenSize.H - s.H : y;
			}

			Thread.Sleep(Instance.Delay + additionalDelay);
		}
	}
}
