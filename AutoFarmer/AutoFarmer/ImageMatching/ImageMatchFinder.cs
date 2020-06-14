using AForge.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoFarmer
{
	public class ImageMatchFinder
	{
		public double Scale { get; set; }

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

		public List<Point> FindClickPointForTemplate(Bitmap sourceImage, ImageFindCondition condition, float similiarityThreshold)
		{
			ImageMatchTemplate template = Templates.First(t => t.Name == condition.TemplateName);

			Bitmap sourceImageConverted = ConvertAndScaleBitmapTo24bpp(sourceImage);
			Bitmap templateImageConverted = ConvertAndScaleBitmapTo24bpp(template.Bitmap);

			SearchRectangle searchRectangle = template.SearchRectangles[condition.SearchRectangleName];

			Rectangle scaledSearchRectangle = ScaleSearchRectangle(searchRectangle);

			Bitmap searchImage = CropTemplateImage(templateImageConverted, scaledSearchRectangle);

			ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(similiarityThreshold);

			TemplateMatch[] matches = matching.ProcessImage(sourceImageConverted, searchImage);

			if (matches.Length == 0) throw new ImageMatchNotFoundException();

			if (matches.Length > condition.MaxAmbiguousity)
			{
				LogAmbiguousException(matches, sourceImage, searchRectangle, condition.TemplateName, condition.SearchRectangleName);

				throw new ImageMatchAmbiguousException(matches.Length);
			}

			List<Point> clickPoints = new List<Point>();

			foreach (var match in matches)
			{
				Rectangle matchRectangle = match.Rectangle;

				Point clickPoint = CalculateClickPoint(ref matchRectangle, searchRectangle.RelativeClickPoint);

				clickPoints.Add(clickPoint);

				Logger.Log($"Match found for {condition.SearchRectangleName} of {condition.TemplateName} at X: {clickPoint.X} Y: {clickPoint.Y}");

				Logger.GraphicalLog(sourceImage, new[] { clickPoint }, new[] { matchRectangle }, condition.TemplateName, condition.SearchRectangleName);
			}

			return clickPoints;
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

		private Point CalculateClickPoint(ref Rectangle rectangle, Size relativeClickPoint)
		{
			rectangle = new Rectangle((int)(rectangle.X / Scale), (int)(rectangle.Y / Scale),
				(int)(rectangle.Width / Scale), (int)(rectangle.Height / Scale));

			Point clickPoint = rectangle.Location + relativeClickPoint;

			Logger.Log($"Click point calculated: {clickPoint} for rectangle: {rectangle}");

			return clickPoint;
		}

		private void LogAmbiguousException(TemplateMatch[] result, Bitmap sourceImage, SearchRectangle searchRectangle, string templateName, string searchRectangleName)
		{
			List<Point> falseClickPoints = new List<Point>();
			List<Rectangle> falseMatchRectangles = new List<Rectangle>();

			foreach (var falseMatch in result)
			{
				var falseMatchRectangle = falseMatch.Rectangle;
				var falseClickPoint = CalculateClickPoint(ref falseMatchRectangle, searchRectangle.RelativeClickPoint);

				falseClickPoints.Add(falseClickPoint);
				falseMatchRectangles.Add(falseMatchRectangle);
			}

			Logger.GraphicalLog(sourceImage, falseClickPoints.ToArray(), falseMatchRectangles.ToArray(), templateName, searchRectangleName);

		}
	}
}
