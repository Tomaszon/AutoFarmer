using System.Drawing;
using System.Drawing.Imaging;

namespace AutoFarmer.Models.ImageMatching
{
	public static class ImageFactory
	{
		public static Bitmap CreateScreenshot(double scale = 1)
		{
			var bmp = new Bitmap(Config.Instance.ScreenSize.W, Config.Instance.ScreenSize.H, PixelFormat.Format24bppRgb);

			using (var g = Graphics.FromImage(bmp))
			{
				g.CopyFromScreen(0, 0, 0, 0, (Size)Config.Instance.ScreenSize, CopyPixelOperation.SourceCopy);
			}

			return ConvertAndScaleBitmap(bmp, scale);
		}

		public static Bitmap ConvertAndScaleBitmap(Bitmap original, double scale = 1)
		{
			Bitmap clone = new Bitmap((int)(original.Width * scale), (int)(original.Height * scale), PixelFormat.Format24bppRgb);

			using (Graphics g = Graphics.FromImage(clone))
			{
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
				g.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
			}

			return clone;
		}
	}
}
