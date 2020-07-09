using AutoFarmer.Models.Common;
using AutoFarmer.Services.Logging;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchTemplate
	{
		private string _bitmapName;

		public string Name { get; set; }

		public Dictionary<string, SearchRectangle> SearchRectangles { get; set; }

		public void Init()
		{
			using var log = Logger.LogBlock();

			foreach (var searchRectangle in SearchRectangles)
			{
				searchRectangle.Value.Init();
			}

			_bitmapName = Directory.GetFiles(Config.Instance.ImageMatchTemplateResourcesDirectory).First(e => Path.GetFileNameWithoutExtension(e) == Name);
		}

		public Bitmap LoadBitmap()
		{
			return (Bitmap)Image.FromFile(_bitmapName);
		}
	}
}
