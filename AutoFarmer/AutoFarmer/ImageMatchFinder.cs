using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace image_search_test
{
	public class ImageMatchFinder
	{
		public string TemplatesRootDirectory { get;
			set; }

		public string TemplateFileExtension { get; set; }

		public double Scale { get;
			set; }

		public float SimiliarityThreshold { get; set; }

		private Dictionary<string, Template> _templates;
		public Dictionary<string, Template> Templates
		{
			get
			{
				return _templates;
			}
			set
			{
				_templates = value;

				foreach (var template in _templates)
				{
					string fileName = Path.Combine(TemplatesRootDirectory, $"{template.Key}.{TemplateFileExtension}");

					template.Value.LoadBitmap(fileName);
				}
			}
		}

		public Point FindClickPointForTemplate(Bitmap sourceImage, Bitmap template, Rectangle searchRectangle)
		{
			Bitmap sourceImageConverted = ConvertAndScaleBitmapTo24bpp(sourceImage);
			Bitmap templateImageConverted = ConvertAndScaleBitmapTo24bpp(template);

			Rectangle scaledSearchRectangle = ScaleSearchRectangle(searchRectangle);

			Bitmap searchImage = CropTemplateImage(templateImageConverted, scaledSearchRectangle);

			ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(SimiliarityThreshold);

			TemplateMatch[] result = matching.ProcessImage(sourceImageConverted, searchImage);

			if (result.Length != 1)
			{
				throw result.Length > 1 ? (Exception)new ImageMatchAmbiguousException(result.Length) : new ImageMatchNotFoundException();
			}

			return CalculateClickPoint(result[0].Rectangle);
		}

		private Bitmap ConvertAndScaleBitmapTo24bpp(Bitmap original)
		{
			Bitmap clone = new Bitmap((int)(original.Width * Scale), (int)(original.Height * Scale), PixelFormat.Format24bppRgb);

			using (Graphics gr = Graphics.FromImage(clone))
			{
				gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
				gr.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
			}

			return clone;
		}

		private Rectangle ScaleSearchRectangle(Rectangle rectangle)
		{
			return new Rectangle((int)(rectangle.X * Scale), (int)(rectangle.Y * Scale),
				(int)(rectangle.Width * Scale), (int)(rectangle.Height * Scale));
		}

		private Bitmap CropTemplateImage(Bitmap bitmap, Rectangle rectangle)
		{
			Bitmap searchImage = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format24bppRgb);

			using (Graphics gr = Graphics.FromImage(searchImage))
			{
				gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
				gr.DrawImage(bitmap, new Rectangle(0, 0, searchImage.Width, searchImage.Height), rectangle, GraphicsUnit.Pixel);
			}

			return searchImage;
		}

		private Point CalculateClickPoint(Rectangle rectangle)
		{
			var backScaledRectangle = new Rectangle((int)(rectangle.X / Scale), (int)(rectangle.Y / Scale),
				(int)(rectangle.Width / Scale), (int)(rectangle.Height / Scale));

			Point center = new Point(backScaledRectangle.X + backScaledRectangle.Width / 2,
				backScaledRectangle.Y + backScaledRectangle.Height / 2);

			return center;
		}
	}
}
