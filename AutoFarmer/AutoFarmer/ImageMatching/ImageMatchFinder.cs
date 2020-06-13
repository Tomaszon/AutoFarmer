﻿using AForge.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AutoFarmer
{
	public class ImageMatchFinder
	{
		public double Scale { get; set; }

		public float SimiliarityThreshold { get; set; }

		public List<ImageMatchTemplate> Templates { get; set; } = new List<ImageMatchTemplate>();

		public static ImageMatchFinder FromConfig()
		{
			var imf = JsonConvert.DeserializeObject<ImageMatchFinder>(File.ReadAllText(Config.Instance.ImageMatchFinderConfigPath));

			foreach (var template in Directory.GetFiles(Config.Instance.ImageMatchTemplatesDirectory))
			{
				imf.Templates.Add(ImageMatchTemplate.FromJsonFile(template, Config.Instance.ImageMatchTemplateResourcesDirectory));
			}

			return imf;
		}

		public Point FindClickPointForTemplate(Bitmap sourceImage, string templateName, string searchRectangleName)
		{
			var template = Templates.First(t => t.Name == templateName);

			Bitmap sourceImageConverted = ConvertAndScaleBitmapTo24bpp(sourceImage);
			Bitmap templateImageConverted = ConvertAndScaleBitmapTo24bpp(template.Bitmap);

			var searchRectangle = template.SearchRectangles[searchRectangleName];

			Rectangle scaledSearchRectangle = ScaleSearchRectangle(searchRectangle);

			Bitmap searchImage = CropTemplateImage(templateImageConverted, scaledSearchRectangle);

			ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(SimiliarityThreshold);

			TemplateMatch[] result = matching.ProcessImage(sourceImageConverted, searchImage);

			if (result.Length != 1)
			{
				throw result.Length > 1 ? (Exception)new ImageMatchAmbiguousException(result.Length) : new ImageMatchNotFoundException();
			}

			Logger.Log($"Match found for {searchRectangleName} of {templateName}, X:{searchRectangle.X} Y:{searchRectangle.Y}");

			var clickPoint = CalculateClickPoint(result[0].Rectangle, searchRectangle.RelativeClickPoint); ;

			Logger.GraphicalLog(sourceImage, clickPoint, searchRectangle, templateName, searchRectangleName);

			return clickPoint;
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

		private Rectangle ScaleSearchRectangle(SearchRectangle rectangle)
		{
			return new Rectangle((int)(rectangle.X * Scale), (int)(rectangle.Y * Scale),
				(int)(rectangle.W * Scale), (int)(rectangle.H * Scale));
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

		private Point CalculateClickPoint(Rectangle rectangle, Size relativeClickPoint)
		{
			var backScaledRectangle = new Rectangle((int)(rectangle.X / Scale), (int)(rectangle.Y / Scale),
				(int)(rectangle.Width / Scale), (int)(rectangle.Height / Scale));

			Point clickPoint = backScaledRectangle.Location + relativeClickPoint;

			Logger.Log($"Click point calculated: {clickPoint} for rectangle: {backScaledRectangle}");

			return clickPoint;
		}
	}
}
