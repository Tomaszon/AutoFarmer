﻿using AutoFarmer.Models.Common;
using AutoFarmer.Services.Logging;
using System.Drawing;
using System.Drawing.Imaging;

namespace AutoFarmer.Services.Imaging
{
	public static class ImageFactory
	{
		public static Bitmap CreateScreenshot()
		{
			Logger.Log("Create screenshot");

			var bmp = new Bitmap(Config.Instance.ScreenSize.W, Config.Instance.ScreenSize.H, PixelFormat.Format24bppRgb);

			using (var g = Graphics.FromImage(bmp))
			{
				g.CopyFromScreen((Point)Config.Instance.ScreenLocation, new Point(), (Size)Config.Instance.ScreenSize, CopyPixelOperation.SourceCopy);
			}

			return bmp;
		}

		public static Bitmap ConvertBitmap(Bitmap original)
		{
			Bitmap clone = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);

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
