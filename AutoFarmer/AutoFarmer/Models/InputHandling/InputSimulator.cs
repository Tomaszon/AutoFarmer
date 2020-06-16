using System;
using System.Drawing;
using System.Threading;
using WindowsInput.Native;

namespace AutoFarmer.Models.InputHandling
{
	public class InputSimulator
	{
		public int Delay { get; set; } = 250;

		private readonly WindowsInput.InputSimulator _simulator;

		public Size ScreenSize { get; set; } = new Size(1920, 1080);

		public InputSimulator()
		{
			_simulator = new WindowsInput.InputSimulator();
		}

		public void Simulate(string[] inputActionNames, Point actionPosition, int additionalDelay = 0)
		{
			foreach (var action in inputActionNames)
			{
				MouseSafetyMeasures.Instance.CheckForIntentionalEmergencyStop(actionPosition);

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

		public void KeyboardEvent(VirtualKeyCode virtualKeyCode, int additionalDelay = 0)
		{
			_simulator.Keyboard.KeyPress(virtualKeyCode);

			Logger.Log($"Virtual key pressed: {virtualKeyCode}", NotificationType.Click);

			Thread.Sleep(Delay + additionalDelay);
		}

		public void MouseEvent(MouseAction mouseAction, int additionalDelay = 0)
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
				{
					_simulator.Mouse.LeftButtonDown();

					Thread.Sleep(1000);

					_simulator.Mouse.LeftButtonUp();

					break;
				}
				case MouseAction.LeftHold5sec:
				{
					_simulator.Mouse.LeftButtonDown();

					Thread.Sleep(5000);

					_simulator.Mouse.LeftButtonUp();

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
		public void MouseEvent(Point point, int additionalDelay = 0)
		{
			_simulator.Mouse.MoveMouseTo(point.X * (65536.0 / ScreenSize.Width) + 1, point.Y * (65536.0 / ScreenSize.Height) + 1);

			Logger.Log($"Mouse move to: {point}");

			Thread.Sleep(Delay + additionalDelay);
		}

		public void ScanScreenForFadedUI(int delay = 100)
		{
			for (int y = 0; y < ScreenSize.Height; y += 50)
			{
				for (int x = 0; x < ScreenSize.Width; x += 250)
				{
					MouseEvent(new Point(x, y), Delay * -1 + delay);
				}
			}
		}
	}
}
