using System.Drawing;
using System.Drawing.Imaging;

namespace AutoFarmer
{
	public static class ScreenshotMaker
	{
		public static Bitmap CreateScreenshot()
		{
			var bmp = new Bitmap(1920, 1080, PixelFormat.Format24bppRgb);

			using (var g = Graphics.FromImage(bmp))
			{
				g.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080), CopyPixelOperation.SourceCopy);
			}

			return bmp;
		}
	}
}
