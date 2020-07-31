using AutoFarmer.Models.Common;
using AutoFarmer.Services.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using WindowsInput.Native;

namespace AutoFarmer.Services.InputHandling
{
	public class InputSimulator
	{
		public int Delay { get; set; } = 250;

		public SerializableSize ScreenSize { get; set; }

		public static InputSimulator Instance { get; set; } = null!;

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

			MouseSafetyMeasures.Instance.LastActionPosition = actionPosition ?? MouseSafetyMeasures.Instance.LastActionPosition;

			foreach (var action in inputActionNames)
			{
				MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

				if (IsSpecialInput(action, out var type, out var commandValue))
				{
					if (type == SpecialAction.Move && TryParseSpecial(commandValue, 2, out var values))
					{
						MoveEvent(new SerializablePoint() { X = values[0], Y = values[1] });
					}
					else if (type == SpecialAction.LeftHold && TryParseSpecial(commandValue, 1, out values))
					{
						if (actionPosition is { })
						{
							HoldEvent(values[0]);
						}
					}
					else if (type == SpecialAction.ScanScreenForFadedUI && TryParseSpecial(commandValue, 3, out values))
					{
						ScanEvent(values[0], values[1], values[2], additionalDelay);
					}
					else if (type == SpecialAction.Multiply && TryParseMultiply(commandValue, out int value, out string variableName))
					{
						if (actionPosition is { })
						{
							MultiplyKeyboardEvent(variableName, value);
						}
					}
					else
					{
						throw new AutoFarmerException($"Unknown special input action: {action}");
					}
				}
				else if (Enum.TryParse<MouseAction>(action, true, out var mouseAction))
				{
					if (actionPosition is { })
					{
						ClickEvent(mouseAction, additionalDelay);
					}
				}
				else if (Enum.TryParse<VirtualKeyCode>(action, true, out var keyboardAction))
				{
					if (actionPosition is { })
					{
						KeyboardEvent(keyboardAction, additionalDelay);
					}
				}
				else
				{
					throw new AutoFarmerException($"Unknown input action: {action}");
				}
			}
		}

		private static List<VirtualKeyCode> ConvertValueToVirtualKeyCode(int value)
		{
			var result = new List<VirtualKeyCode>();

			foreach (var c in value.ToString())
			{
				result.Add((VirtualKeyCode)(int.Parse(c.ToString()) + 96));
			}

			return result;
		}

		private static bool IsSpecialInput(string value, out SpecialAction specialActionType, out string commandValue)
		{
			specialActionType = default;
			commandValue = default;

			Regex regex = new Regex("(?<type>\\w+):(?<value>(\\w|,)+)");

			var result = regex.Match(value);

			if (result.Success && Enum.TryParse(result.Groups["type"].Value, out specialActionType))
			{
				commandValue = result.Groups["value"].Value;

				return true;
			}

			return false;
		}

		private static bool TryParseMultiply(string commandValue, out int value, out string variableName)
		{
			value = default;
			variableName = default;

			Regex regex = new Regex("(?<value>(\\d)+),(?<variable>(\\w)+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var match = regex.Match(commandValue);

			if (match.Success && int.TryParse(match.Groups["value"].Value, out value))
			{
				variableName = match.Groups["variable"].Value;

				return true;
			}

			return false;
		}

		private static bool TryParseSpecial(string commandValue, int expectedCount, out List<int> values)
		{
			values = new List<int>();

			Regex regex = new Regex("\\d+");

			var matches = regex.Matches(commandValue);

			if (matches.Count == expectedCount)
			{
				for (int i = 0; i < matches.Count; i++)
				{
					if (int.TryParse(matches[i].Value, out var v))
					{
						values.Add(v);
					}
				}

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
		public static void MoveEvent(SerializablePoint point, int additionalDelay = 0)
		{
			using var log = Logger.LogBlock();

			Logger.Log($"Mouse move to: {point}");

			SmoothMouseMove(point);

			MouseSafetyMeasures.Instance.LastActionPosition = MouseSafetyMeasures.GetCursorCurrentPosition();

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		public static void ScanEvent(int stepSize, int scanSize, int delay, int additionalDelay = 0)
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

		private static void MultiplyKeyboardEvent(string variableName, int value, int additionalDelay = 0)
		{
			var variable = GlobalStateStorage.Get(variableName);

			int multipliedValue = value * variable.Value;

			var kcs = ConvertValueToVirtualKeyCode(multipliedValue);

			kcs.ForEach(c => KeyboardEvent(c, additionalDelay));

			variable.Increase();
		}

		private static void SmoothMouseMove(SerializablePoint to, int stepSize = 75, int delay = 10)
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
