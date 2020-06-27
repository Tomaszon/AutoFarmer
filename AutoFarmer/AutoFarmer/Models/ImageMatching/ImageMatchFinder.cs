using AForge.Imaging;
using AutoFarmer.Models.GraphNamespace;
using Newtonsoft.Json;
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

		public float SimiliarityThresholdCorrectionOnScaling { get; set; }

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

		public static List<SerializablePoint> FindClickPointForTemplate(MatchCondition condition, Bitmap source, float similiarityThreshold)
		{
			var matchResult = CalculateMatches(source, condition, similiarityThreshold);

			Logger.GraphicalLog(matchResult, condition.TemplateName, condition.SearchRectangleName);

			if (matchResult.Matches.Count < condition.MinimumOccurrence)
			{
				throw new ImageMatchNotFoundException(_performanceMonitor.Elapsed);
			}

			if (matchResult.Matches.Count > condition.MaximumOccurrence)
			{
				throw new ImageMatchAmbiguousException(matchResult.Matches.Count(), _performanceMonitor.Elapsed);
			}

			return OrderResults(matchResult.Matches.Select(m => m.ClickPoint), condition.OrderBy);
		}

		private static ImageMatchResult CalculateMatches(Bitmap source, MatchCondition condition, float similiarityThreshold)
		{
			var template = Instance.Templates.Single(t => t.Name == condition.TemplateName);
			var searchRectangle = template.SearchRectangles[condition.SearchRectangleName];

			var templateImage = ImageFactory.ConvertBitmap(template.Bitmap);
			var searchImage = CropTemplateImage(templateImage, searchRectangle);

			var result = new ImageMatchResult()
			{
				SearchAreas = searchRectangle.SearchAreas,
				Source = source,
				SearchImage = searchImage
			};

			_performanceMonitor.Start();

			foreach (var searchArea in result.SearchAreas)
			{
				result.Matches.AddRange(CollectMatches(source, searchImage, searchRectangle.RelativeClickPoint, similiarityThreshold, condition.SearchRectangleName, condition.TemplateName, searchArea));

				if (result.Matches.Count > condition.MaximumOccurrence && !(Config.Instance.GraphicalLogging && Config.Instance.FileLogging)) break;
			}

			if (result.Matches.Count < condition.MinimumOccurrence)
			{
				Logger.Log($"Search in given search areas not resulted minimum {condition.MinimumOccurrence} matches. Searching in full image!");

				result.Matches = CollectMatches(source, searchImage, searchRectangle.RelativeClickPoint, similiarityThreshold, condition.SearchRectangleName, condition.TemplateName);
			}

			_performanceMonitor.Stop();

			Logger.Log($"Mathes calculated. Full search time: {_performanceMonitor.Elapsed}");

			return result;
		}

		private static List<ImageMatch> CollectMatches(Bitmap source, Bitmap searchImage, SerializableSize relativeClickPoint, float similiarityThreshold, string searchRectangleName, string templateName, SerializableRectangle area = null)
		{
			ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(similiarityThreshold);

			var findResult = area is null ? matching.ProcessImage(source, searchImage) : matching.ProcessImage(source, searchImage, (Rectangle)area);

			var result = findResult.Select(tm =>
				new ImageMatch((SerializablePoint)tm.Rectangle.Location + relativeClickPoint, (SerializableRectangle)tm.Rectangle)).ToList();

			result.ForEach(m => Logger.Log($"Match found for {searchRectangleName} of {templateName}{(area is null ? "" : $" in {area} search area") } at {m.ClickPoint}. At search time: {_performanceMonitor.Elapse}"));

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
