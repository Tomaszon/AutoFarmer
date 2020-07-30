using AutoFarmer.Services;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchTemplate : ImageMatchTemplateBase
	{
		private readonly string _bitmapName;

		public Dictionary<string, SearchRectangle> SearchRectangles { get; set; }

		public ImageMatchTemplate(string name, Dictionary<string, SearchRectangle> searchRectangles) : base(name)
		{
			_bitmapName = Path.Combine(Config.Instance.ImageMatchTemplateResourcesDirectory, $"{name}.png");
			SearchRectangles = searchRectangles;
		}

		public Bitmap GetBitmap()
		{
			return (Bitmap)Image.FromFile(_bitmapName);
		}
	}
}
