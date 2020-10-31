using AutoFarmer.Models.Common;
using AutoFarmer.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;
using WindowsInput.Native;

namespace AutoFarmer.Services.InputHandling
{
	public class InputSimulator
	{
		public int Delay { get; set; } = 250;

		public SerializableSize ScreenSize { get; set; } = null!;

		public static InputSimulator Instance { get; set; } = null!;

		public static void FromConfig()
		{
			Instance = new InputSimulator
			{
				ScreenSize = Config.Instance.ScreenSize
			};
		}

		public static void Simulate(string[] inputActionNames, SerializablePoint? actionPosition, int additionalDelay = 0)
		{
			using var log = Logger.LogBlock();

			if (inputActionNames is null) return;

			MouseSafetyMeasures.Instance.LastActionPosition = actionPosition ?? MouseSafetyMeasures.Instance.LastActionPosition;

			foreach (var action in inputActionNames)
			{
				MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

				if (IsSpecialInput(action, out var type, out var commandValue))
				{
					switch (type)
					{
						case SpecialAction.LeftHold:
						{
							if (!TryParseSpecial(commandValue, 1, out var values)) goto default;

							if (actionPosition is { })
							{
								HoldEvent(values[0]);
							}
						}
						break;
						case SpecialAction.ScanScreenForFadedUI:
						{
							if (!TryParseSpecial(commandValue, 3, out var values)) goto default;

							ScanEvent(values[0], values[1], values[2], additionalDelay);
						}
						break;
						case SpecialAction.Move:
						{
							if (!TryParseSpecial(commandValue, 2, out var values)) goto default;

							MoveEvent(new SerializablePoint() { X = values[0], Y = values[1] });
						}
						break;
						case SpecialAction.Multiply:
						{
							if (!TryParseMultiply(commandValue, out var value1, out var value2)) goto default;

							if (actionPosition is { })
							{
								MultiplyKeyboardEvent(value1, value2);
							}
						}
						break;
						case SpecialAction.Value:
						{
							if (!TryParseValue(commandValue, out var value)) goto default;

							if (actionPosition is { })
							{
								ValueKeyboardEvent(value);
							}
						}
						break;
						default:
						{
							throw new AutoFarmerException($"Unknown special input action: {action}");
						}
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

		private static List<VirtualKeyCode> ConvertValueToVirtualKeyCode(long value)
		{
			var result = new List<VirtualKeyCode>();

			foreach (var c in value.ToString())
			{
				result.Add((VirtualKeyCode)(int.Parse(c.ToString()) + 96));
			}

			return result;
		}

		private static bool IsSpecialInput(string value, out SpecialAction specialActionType, [NotNullWhen(true)] out string? commandValue)
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

		private static bool TryParseValue(string commandValue, [NotNullWhen(true)] out GlobalStateStorageValue? value)
		{
			value = default;

			string formattedGlobalStateValues = FormatArrayForRegexSearch(GlobalStateStorage.GetValueNames(), "|", "|");

			Regex regex = new Regex($"(?<value>(\\d)+{formattedGlobalStateValues})", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var match = regex.Match(commandValue);

			if (match.Success)
			{
				var v = match.Groups["value"].Value;

				value = int.TryParse(v, out var x) ? x : GlobalStateStorage.Get(v);

				return true;
			}

			return false;
		}

		private static bool TryParseMultiply(string commandValue, [NotNullWhen(true)] out GlobalStateStorageValue? value1, [NotNullWhen(true)] out GlobalStateStorageValue? value2)
		{
			value1 = default;
			value2 = default;

			string formattedGlobalStateValues = FormatArrayForRegexSearch(GlobalStateStorage.GetValueNames(), "|", "|");

			Regex regex = new Regex($"(?<value1>(\\d)+{formattedGlobalStateValues}),(?<value2>(\\d)+{formattedGlobalStateValues})", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var match = regex.Match(commandValue);

			if (match.Success)
			{
				var v1 = match.Groups["value1"].Value;
				var v2 = match.Groups["value2"].Value;

				value1 = int.TryParse(v1, out var x) ? x : GlobalStateStorage.Get(v1);
				value2 = int.TryParse(v2, out x) ? x : GlobalStateStorage.Get(v2);

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

		private static void ValueKeyboardEvent(GlobalStateStorageValue value)
		{
			var kcs = ConvertValueToVirtualKeyCode(value.Value);

			kcs.ForEach(c => KeyboardEvent(c, 0));

			value.Increase();
		}

		private static void MultiplyKeyboardEvent(GlobalStateStorageValue value1, GlobalStateStorageValue value2)
		{
			long multipliedValue = value1.Value * value2.Value;

			var kcs = ConvertValueToVirtualKeyCode(multipliedValue);

			kcs.ForEach(c => KeyboardEvent(c, 0));

			value1.Increase();
			value2.Increase();
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

		private static string FormatArrayForRegexSearch(IEnumerable<object> values, string separators, string? begining = "", string? ending = "")
		{
			return begining + string.Join(separators, values) + ending;
		}
	}
}
