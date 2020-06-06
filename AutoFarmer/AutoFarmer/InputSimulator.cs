using System.Drawing;
using System.Threading;
using WindowsInput.Native;

namespace image_search_test
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
			Thread.Sleep(Delay + additionalDelay);
		}

		public void MouseLeftClick(int additionalDelay = 0)
		{
			_simulator.Mouse.LeftButtonClick();
			Thread.Sleep(Delay + additionalDelay);
		}

		public void KeyPress(VirtualKeyCode virtualKeyCode, int additionalDelay = 0)
		{
			_simulator.Keyboard.KeyPress(virtualKeyCode);
			Thread.Sleep(Delay + additionalDelay);
		}
	}
}
