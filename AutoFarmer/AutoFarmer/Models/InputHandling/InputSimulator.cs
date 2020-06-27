using AutoFarmer.Models.Common;
using System;
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
					MouseEvent(mouseAction, additionalDelay);
				}
				else if (Enum.TryParse<VirtualKeyCode>(action, true, out var keyboardAction))
				{
					KeyboardEvent(keyboardAction, additionalDelay);
				}
			}
		}

		private static void KeyboardEvent(VirtualKeyCode virtualKeyCode, int additionalDelay = 0)
		{
			new WindowsInput.InputSimulator().Keyboard.KeyPress(virtualKeyCode);

			Logger.Log($"Virtual key pressed: {virtualKeyCode}", NotificationType.Click);

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		private static void MouseEvent(MouseAction mouseAction, int additionalDelay = 0)
		{
			var simulator = new WindowsInput.InputSimulator();

			switch (mouseAction)
			{
				case MouseAction.LeftClick:
				{
					simulator.Mouse.LeftButtonClick();

					Logger.Log("Left mouse click", NotificationType.Click);
					break;
				}
				case MouseAction.LeftDoubleClick:
				{
					simulator.Mouse.LeftButtonDoubleClick();

					Logger.Log("Left mouse double click", NotificationType.Click, 1);//TODO use unique sound
					break;
				}
				case MouseAction.ScanScreenForFadedUI:
				{
					ScanScreenForFadedUI();

					Logger.Log("Scanning for faded UI");
					break;
				}
				case MouseAction.LeftHold1sec:
				case MouseAction.LeftHold5sec:
				{
					simulator.Mouse.LeftButtonDown();

					Logger.Log("Left mouse hold", NotificationType.Click, 1);//TODO use unique sound

					Thread.Sleep((int)mouseAction);

					simulator.Mouse.LeftButtonUp();

					Logger.Log("Left mouse release", NotificationType.Click, 1);//TODO use unique sound

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
		public static void MoveMouseTo(SerializablePoint point, int additionalDelay = 0)
		{
			MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

			new WindowsInput.InputSimulator().Mouse.MoveMouseTo(point.X * (65536.0 / Instance.ScreenSize.W) + 1, point.Y * (65536.0 / Instance.ScreenSize.H) + 1);

			Logger.Log($"Mouse move to: {point}");

			Thread.Sleep(Instance.Delay + additionalDelay);
		}

		public static void ScanScreenForFadedUI(int delay = 100)
		{
			for (int y = 0; y < Instance.ScreenSize.H; y += 50)
			{
				for (int x = 0; x < Instance.ScreenSize.W; x += 250)
				{
					MoveMouseTo(new SerializablePoint() { X = x, Y = y }, Instance.Delay * -1 + delay);
				}
			}
		}
	}
}
