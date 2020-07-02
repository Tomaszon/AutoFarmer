﻿using AutoFarmer.Models.Common;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using WindowsInput.Native;

namespace AutoFarmer.Models.InputHandling
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
			MouseSafetyMeasures.Instance.LastActionPosition = actionPosition;

			foreach (var action in inputActionNames)
			{
				MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

				if (Enum.TryParse<MouseAction>(action, true, out var mouseAction))
				{
					ClickEvent(mouseAction, additionalDelay);
				}
				else if (Enum.TryParse<VirtualKeyCode>(action, true, out var keyboardAction))
				{
					KeyboardEvent(keyboardAction, additionalDelay);
				}
				else if (TryParse(action, out SerializablePoint p))
				{
					MoveMouseTo(p);
				}
				else
				{
					throw new AutoFarmerException($"Unknown input action: {action}");
				}
			}
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
				case MouseAction.ScanScreenForFadedUI:
				{
					Logger.Log("Scanning for faded UI");

					ScanScreenForFadedUI();

					break;
				}
				case MouseAction.LeftHold5of10sec:
				case MouseAction.LeftHold1sec:
				case MouseAction.LeftHold5sec:
				{
					Logger.Log("Left mouse hold", NotificationType.Click, 1);//TODO use unique sound

					simulator.Mouse.LeftButtonDown();

					Thread.Sleep((int)mouseAction);

					Logger.Log("Left mouse release", NotificationType.Click, 1);//TODO use unique sound

					simulator.Mouse.LeftButtonUp();

					break;
				}
			}

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		/// <summary>
		/// Moves the cursor to the given position.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="additionalDelay"></param>
		public static void MoveMouseTo(SerializablePoint point, int additionalDelay = 0, bool log = true)
		{
			new WindowsInput.InputSimulator().Mouse.MoveMouseTo(point.X * (65536.0 / Instance.ScreenSize.W) + 1, point.Y * (65536.0 / Instance.ScreenSize.H) + 1);

			if (log) Logger.Log($"Mouse move to: {point}");

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		public static void ScanScreenForFadedUI(int delay = 15)
		{
			for (int y = 0; y < Instance.ScreenSize.H; y += 75)
			{
				for (int x = 0; x < Instance.ScreenSize.W; x += 250)
				{
					MoveMouseTo(new SerializablePoint() { X = x, Y = y }, Instance.Delay * -1 + delay, false);
				}
			}

			MoveMouseTo(MouseSafetyMeasures.Instance.LastActionPosition, log: false);
		}
	}
}
