using AForge.Imaging.Filters;
using System;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput.Native;

namespace AutoFarmer
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

		public void MoveMouse(Point point, int additionalDelay = 0)
		{
			_simulator.Mouse.MoveMouseTo(point.X * (65536.0 / ScreenSize.Width), point.Y * (65536.0 / ScreenSize.Height));
			
			Logger.Log($"Mouse move to: {point}");

			Thread.Sleep(Delay + additionalDelay);
		}

		public void MouseLeftClick(int additionalDelay = 0)
		{
			_simulator.Mouse.LeftButtonClick();

			Logger.Log("Left mouse click", NotificationType.Click);

			Thread.Sleep(Delay + additionalDelay);
		}

		public void MouseDoubleLeftClick(int additionalDelay = 0)
		{
			_simulator.Mouse.LeftButtonDoubleClick();

			Logger.Log("Left mouse double click", NotificationType.Click, 2);

			Thread.Sleep(Delay + additionalDelay);
		}

		public void KeyPress(VirtualKeyCode virtualKeyCode, int additionalDelay = 0)
		{
			_simulator.Keyboard.KeyPress(virtualKeyCode);

			Logger.Log($"Virtual key pressed: {virtualKeyCode}", NotificationType.Click);
			
			Thread.Sleep(Delay + additionalDelay);
		}

		public void Simulate(SuccessActions[] successes, int additionalDelay = 0)
		{
			foreach (var success in successes)
			{
				Simulate(success.MouseActions, additionalDelay);
				Simulate(success.KeyboardActions, additionalDelay);
			}
		}

		public void Simulate(VirtualKeyCode[] virtualKeyCodes, int additionalDelay = 0)
		{
			if (virtualKeyCodes != null)
			{
				foreach (var keyCode in virtualKeyCodes)
				{
					KeyPress(keyCode, additionalDelay);
				}
			}
		}

		public void Simulate(MouseAction[] mouseActions, int additionalDelay = 0)
		{
			if (mouseActions != null)
			{
				foreach (var mouseAction in mouseActions)
				{
					switch (mouseAction)
					{
						case MouseAction.LeftClick:
							MouseLeftClick(additionalDelay);
							break;
						case MouseAction.LeftDoubleClick:
							MouseDoubleLeftClick(additionalDelay);
							break;
					}
				}
			}
		}
	}
}
