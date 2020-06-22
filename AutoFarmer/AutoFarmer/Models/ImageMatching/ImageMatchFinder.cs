﻿using AForge.Imaging;
using AutoFarmer.Models.GraphNamespace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchFinder
	{
		private static readonly PerformanceMonitor _performanceMonitor = new PerformanceMonitor();

		public double Scale { get; set; }

		public float DefaultMaximumSimiliarityThreshold { get; set; }

		public float DefaultMiniumuSimiliarityThreshold { get; set; }

		public float DefaultSimiliarityThresholdStep { get; set; }

		public List<ImageMatchTemplate> Templates { get; set; } = new List<ImageMatchTemplate>();

		public static ImageMatchFinder Instance { get; set; }

		public static void FromConfig()
		{
			Instance = JsonConvert.DeserializeObject<ImageMatchFinder>(File.ReadAllText(Config.Instance.ImageMatchFinderConfigPath));

			foreach (var template in Directory.GetFiles(Config.Instance.ImageMatchTemplatesDirectory))
			{
				Instance.Templates.Add(ImageMatchTemplate.FromJsonFile(template, Config.Instance.ImageMatchTemplateResourcesDirectory));
			}
		}

		public static List<Point> FindClickPointForTemplate(MatchCondition condition, Bitmap sourceImage, float similiarityThreshold)
		{
			var matches = CalculateMatches(sourceImage, condition, similiarityThreshold, out var searchRectangle, out var searchAreas);

			if (matches.Length < condition.MinimumOccurrence)
			{
				throw new ImageMatchNotFoundException(_performanceMonitor.Elapsed);
			}

			if (matches.Length > condition.MaximumOccurrence)
			{
				LogAmbiguousException(matches, sourceImage, searchRectangle, condition.TemplateName, condition.SearchRectangleName, searchAreas);

				throw new ImageMatchAmbiguousException(matches.Length, _performanceMonitor.Elapsed);
			}

			List<Point> clickPoints = new List<Point>();

			foreach (var match in matches)
			{
				Rectangle matchRectangle = match.Rectangle;

				Point clickPoint = CalculateClickPoint(ref matchRectangle, searchRectangle.RelativeClickPoint);

				clickPoints.Add(clickPoint);

				Logger.Log($"Match found for {condition.SearchRectangleName} of {condition.TemplateName} at {clickPoint}. Search time: {_performanceMonitor.Elapsed}");

				Logger.GraphicalLog(sourceImage, new[] { clickPoint }, new[] { matchRectangle }, condition.TemplateName, condition.SearchRectangleName, searchAreas);
			}

			return OrderResults(clickPoints, condition.OrderBy);
		}

		private static TemplateMatch[] CalculateMatches(Bitmap sourceImage, MatchCondition condition, float similiarityThreshold, out SearchRectangle searchRectangle, out List<SerializableRectangle> searchAreas)
		{
			ImageMatchTemplate template = Instance.Templates.First(t => t.Name == condition.TemplateName);

			searchRectangle = template.SearchRectangles[condition.SearchRectangleName];

			Bitmap sourceImageConverted = ConvertAndScaleBitmapTo24bpp(sourceImage);

			Bitmap templateImageConverted = ConvertAndScaleBitmapTo24bpp(template.Bitmap);
			Rectangle scaledSearchRectangle = ScaleSearchRectangle(searchRectangle);
			Bitmap searchImage = CropTemplateImage(templateImageConverted, scaledSearchRectangle);

			ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(similiarityThreshold);

			_performanceMonitor.Start();

			TemplateMatch[] matches = Array.Empty<TemplateMatch>();

			searchAreas = CalculateSearchArea(searchRectangle);

			foreach (var searchArea in searchAreas)
			{
				matches = matching.ProcessImage(sourceImageConverted, searchImage, searchArea);

				if (matches.Length != 0) break;
			}

			_performanceMonitor.Stop();

			return matches;
		}

		private static List<SerializableRectangle> CalculateSearchArea(SearchRectangle searchRectangle)
		{
			var areas = new List<SerializableRectangle>();

			if (searchRectangle.SearchArea != null || searchRectangle.NamedSearchAreas != null)
			{
				if (searchRectangle.SearchArea != null) areas.Add(searchRectangle.SearchArea);

				if (searchRectangle.NamedSearchAreas != null)
				{
					foreach (var namedSearchArea in searchRectangle.NamedSearchAreas)
					{
						switch (namedSearchArea)
						{
							case NamedSearchArea.Left:
								areas.Add(SearchArea.Left(searchRectangle.W));

								break;
							case NamedSearchArea.Right:
								areas.Add(SearchArea.Right(searchRectangle.W));

								break;
							case NamedSearchArea.Upper:
								areas.Add(SearchArea.Upper(searchRectangle.H));

								break;
							case NamedSearchArea.Lower:
								areas.Add(SearchArea.Lower(searchRectangle.H));

								break;
							case NamedSearchArea.UpperLeft:
								areas.Add(SearchArea.UpperLeft(searchRectangle.W, searchRectangle.H));

								break;
							case NamedSearchArea.UpperRight:
								areas.Add(SearchArea.UpperRight(searchRectangle.W, searchRectangle.H));

								break;
							case NamedSearchArea.LowerLeft:
								areas.Add(SearchArea.LowerLeft(searchRectangle.W, searchRectangle.H));

								break;
							case NamedSearchArea.LowerRight:
								areas.Add(SearchArea.LowerRight(searchRectangle.W, searchRectangle.H));

								break;
						}
					}
				}
			}
			else
			{
				switch (searchRectangle.AutoSearchAreaMode)
				{
					case AutoSearchAreaMode.LeftRight:
					{
						areas.Add(SearchArea.Left(searchRectangle.W));
						areas.Add(SearchArea.Right(searchRectangle.W));

						break;
					}
					case AutoSearchAreaMode.UpperLower:
					{
						areas.Add(SearchArea.Upper(searchRectangle.H));
						areas.Add(SearchArea.Lower(searchRectangle.H));

						break;
					}
					case AutoSearchAreaMode.Quarter:
					{
						areas.Add(SearchArea.UpperLeft(searchRectangle.W, searchRectangle.H));
						areas.Add(SearchArea.UpperRight(searchRectangle.W, searchRectangle.H));
						areas.Add(SearchArea.LowerLeft(searchRectangle.W, searchRectangle.H));
						areas.Add(SearchArea.LowerRight(searchRectangle.W, searchRectangle.H));

						break;
					}
				}
			}

			return areas;
		}

		private static Bitmap ConvertAndScaleBitmapTo24bpp(Bitmap original)
		{
			Bitmap clone = new Bitmap((int)(original.Width * Instance.Scale), (int)(original.Height * Instance.Scale), PixelFormat.Format24bppRgb);

			using (Graphics gr = Graphics.FromImage(clone))
			{
				gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
				gr.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
			}

			return clone;
		}

		private static Rectangle ScaleSearchRectangle(SearchRectangle rectangle)
		{
			return new Rectangle((int)(rectangle.X * Instance.Scale), (int)(rectangle.Y * Instance.Scale),
				(int)(rectangle.W * Instance.Scale), (int)(rectangle.H * Instance.Scale));
		}

		private static Bitmap CropTemplateImage(Bitmap bitmap, Rectangle rectangle)
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

		private static Point CalculateClickPoint(ref Rectangle rectangle, Size relativeClickPoint)
		{
			rectangle = new Rectangle((int)(rectangle.X / Instance.Scale), (int)(rectangle.Y / Instance.Scale),
				(int)(rectangle.Width / Instance.Scale), (int)(rectangle.Height / Instance.Scale));

			Point clickPoint = rectangle.Location + relativeClickPoint;

			Logger.Log($"Click point calculated: {clickPoint} for rectangle: {rectangle}");

			return clickPoint;
		}

		private static void LogAmbiguousException(TemplateMatch[] result, Bitmap sourceImage, SearchRectangle searchRectangle, string templateName, string searchRectangleName, List<SerializableRectangle> searchAreas)
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

			Logger.GraphicalLog(sourceImage, falseClickPoints.ToArray(), falseMatchRectangles.ToArray(), templateName, searchRectangleName, searchAreas);
		}

		private static List<Point> OrderResults(List<Point> points, Dictionary<MatchOrderBy, MatchOrderLike> orderBy)
		{
			if (orderBy is null) return points;

			var result = orderBy.TryGetValue(MatchOrderBy.X, out var orderLike) && orderLike == MatchOrderLike.Descending
				? points.OrderByDescending(e => e.X) : points.OrderBy(e => e.X);

			result = orderBy.TryGetValue(MatchOrderBy.Y, out orderLike) && orderLike == MatchOrderLike.Descending
				? result.ThenByDescending(e => e.Y) : result.ThenBy(e => e.Y);

			return result.ToList();
		}
	}
}
