using AutoFarmer.Models.Common;
using System;
using System.Threading;
using WindowsInput.Native;

namespace AutoFarmer.Models.InputHandling
{
	public class InputSimulator
	{
		public int Delay { get; set; } = 250;

		private readonly WindowsInput.InputSimulator _simulator = new WindowsInput.InputSimulator();

		public SerializableSize ScreenSize { get; set; }

		public static InputSimulator Instance { get; set; }

		public static void FromConfig()
		{
			Instance = new InputSimulator
			{
				ScreenSize = Config.Instance.ScreenSize
			};
		}

		public void Simulate(string[] inputActionNames, SerializablePoint actionPosition, int additionalDelay = 0)
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

		private void KeyboardEvent(VirtualKeyCode virtualKeyCode, int additionalDelay = 0)
		{
			_simulator.Keyboard.KeyPress(virtualKeyCode);

			Logger.Log($"Virtual key pressed: {virtualKeyCode}", NotificationType.Click);

			Thread.Sleep(Delay + additionalDelay);
		}

		private void MouseEvent(MouseAction mouseAction, int additionalDelay = 0)
		{
			switch (mouseAction)
			{
				case MouseAction.LeftClick:
				{
					_simulator.Mouse.LeftButtonClick();

					Logger.Log("Left mouse click", NotificationType.Click);
					break;
				}
				case MouseAction.LeftDoubleClick:
				{
					_simulator.Mouse.LeftButtonDoubleClick();

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
					_simulator.Mouse.LeftButtonDown();

					Logger.Log("Left mouse hold", NotificationType.Click, 1);//TODO use unique sound

					Thread.Sleep((int)mouseAction);

					_simulator.Mouse.LeftButtonUp();

					Logger.Log("Left mouse release", NotificationType.Click, 1);//TODO use unique sound

					break;
				}
			}

			Thread.Sleep(Delay + additionalDelay);
		}

		/// <summary>
		/// Moves the cursor to the given position.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="additionalDelay"></param>
		public void MoveMouseTo(SerializablePoint point, int additionalDelay = 0)
		{
			MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

			_simulator.Mouse.MoveMouseTo(point.X * (65536.0 / ScreenSize.W) + 1, point.Y * (65536.0 / ScreenSize.H) + 1);

			Logger.Log($"Mouse move to: {point}");

			Thread.Sleep(Delay + additionalDelay);
		}

		public void ScanScreenForFadedUI(int delay = 100)
		{
			for (int y = 0; y < ScreenSize.H; y += 50)
			{
				for (int x = 0; x < ScreenSize.W; x += 250)
				{
					MoveMouseTo(new SerializablePoint() { X = x, Y = y }, Delay * -1 + delay);
				}
			}
		}
	}
}
