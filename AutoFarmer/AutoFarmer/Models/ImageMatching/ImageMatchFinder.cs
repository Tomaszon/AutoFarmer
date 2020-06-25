using AForge.Imaging;
using AutoFarmer.Models.GraphNamespace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

		public static List<SerializablePoint> FindClickPointForTemplate(MatchCondition condition, Bitmap scaledSource, float similiarityThreshold)
		{
			var matchCollection = CalculateMatches(scaledSource, condition, similiarityThreshold);

			if (matchCollection.Matches.Count < condition.MinimumOccurrence)
			{
				throw new ImageMatchNotFoundException(_performanceMonitor.Elapsed);
			}

			if (matchCollection.Matches.Count > condition.MaximumOccurrence)
			{
				Logger.GraphicalLog(matchCollection, condition.TemplateName, condition.SearchRectangleName);

				throw new ImageMatchAmbiguousException(matchCollection.Matches.Count(), _performanceMonitor.Elapsed);
			}

			Logger.GraphicalLog(matchCollection, condition.TemplateName, condition.SearchRectangleName);

			return OrderResults(matchCollection.Matches.Select(m => m.ClickPoint), condition.OrderBy);
		}

		private static ImageMatchCollection CalculateMatches(Bitmap scaledSource, MatchCondition condition, float similiarityThreshold)
		{
			var template = Instance.Templates.First(t => t.Name == condition.TemplateName);
			var scaledTemplate = ImageFactory.ConvertAndScaleBitmap(template.Bitmap, Instance.Scale);
			var searchRectangle = template.SearchRectangles[condition.SearchRectangleName];
			var scaledSearchRectangle = searchRectangle.Scale(Instance.Scale);
			var scaledSearchImage = CropTemplateImage(scaledTemplate, scaledSearchRectangle);

			var result = new ImageMatchCollection()
			{
				ScaledSearchAreas = scaledSearchRectangle.SearchAreas,
				SearchAreas = searchRectangle.SearchAreas,
				ScaledSource = scaledSource
			};

			_performanceMonitor.Start();

			foreach (var searchArea in result.SearchAreas)
			{
				result.Matches.AddRange(CollectMatches(scaledSource, scaledSearchImage, searchRectangle.RelativeClickPoint, similiarityThreshold, condition.SearchRectangleName, condition.TemplateName, searchArea));

				if (result.Matches.Count > condition.MaximumOccurrence && !(Config.Instance.GraphicalLogging && Config.Instance.FileLogging)) break;
			}

			if (result.Matches.Count < condition.MinimumOccurrence)
			{
				result.Matches = CollectMatches(scaledSource, scaledSearchImage, searchRectangle.RelativeClickPoint, similiarityThreshold, condition.SearchRectangleName, condition.TemplateName);
			}

			_performanceMonitor.Stop();

			return result;
		}

		private static List<ImageMatch> CollectMatches(Bitmap scaledSource, Bitmap scaledSearchImage, SerializableSize relativeClickPoint, float similiarityThreshold, string searchRectangleName, string templateName, SerializableRectangle area = null)
		{
			ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(similiarityThreshold);

			var findResult = area is null ? matching.ProcessImage(scaledSource, scaledSearchImage) : matching.ProcessImage(scaledSource, scaledSearchImage, (Rectangle)area);

			var result = findResult.Select(tm =>
				new ImageMatch((SerializablePoint)tm.Rectangle.Location + relativeClickPoint, (SerializableRectangle)tm.Rectangle, 1 / Instance.Scale)).ToList();

			result.ForEach(m => Logger.Log($"Match found for {searchRectangleName} of {templateName} {(area is null ? "" : $"in {area} search area") } at {m.ClickPoint}. Search time: {_performanceMonitor.Elapsed}"));

			return result;
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

		private static List<SerializablePoint> OrderResults(IEnumerable<SerializablePoint> points, Dictionary<MatchOrderBy, MatchOrderLike> orderBy)
		{
			if (orderBy is null) return points.ToList();

			var result = orderBy.TryGetValue(MatchOrderBy.X, out var orderLike) && orderLike == MatchOrderLike.Descending
				? points.OrderByDescending(e => e.X) : points.OrderBy(e => e.X);

			result = orderBy.TryGetValue(MatchOrderBy.Y, out orderLike) && orderLike == MatchOrderLike.Descending
				? result.ThenByDescending(e => e.Y) : result.ThenBy(e => e.Y);

			return result.ToList();
		}
	}
}
