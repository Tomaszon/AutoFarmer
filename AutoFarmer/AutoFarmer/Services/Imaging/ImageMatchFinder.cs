using AForge.Imaging;
using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph.Conditions;
using AutoFarmer.Models.ImageMatching;
using AutoFarmer.Services.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace AutoFarmer.Services.Imaging
{
	public class ImageMatchFinder
	{
		private static readonly PerformanceMonitor _PERFORMANCE_MONITOR = new PerformanceMonitor();

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
				var templateOptions = ImageMatchTemplateOptions.FromJsonFile(template);

				Instance.Templates.Add(templateOptions.ToImageMatchTemplate());
			}
		}

		public static List<SerializablePoint> FindClickPointForTemplate(Condition condition, Bitmap source, float similiarityThreshold)
		{
			using var log = Logger.LogBlock();

			var matchResult = CalculateMatches(source, condition, similiarityThreshold);

			if (matchResult.Matches.Count < condition.MinimumOccurrence)
			{
				throw new ImageMatchNotFoundException(_PERFORMANCE_MONITOR.Elapsed);
			}

			if (matchResult.Matches.Count > condition.MaximumOccurrence)
			{
				throw new ImageMatchAmbiguousException(matchResult.Matches.Count(), _PERFORMANCE_MONITOR.Elapsed);
			}

			return OrderResults(matchResult.Matches.Select(m => m.ClickPoint), condition.OrderBy);
		}

		private static ImageMatchResult CalculateMatches(Bitmap source, Condition condition, float similiarityThreshold)
		{
			using var log = Logger.LogBlock();

			var template = Instance.Templates.Single(t => t.Name == condition.TemplateName);
			var searchRectangle = template.SearchRectangles[condition.SearchRectangleName];

			using var templateImage = ImageFactory.ConvertBitmap(template.GetBitmap());
			using var searchImage = CropTemplateImage(templateImage, (Rectangle)searchRectangle.Rectangle);

			var result = new ImageMatchResult(searchRectangle.SearchAreas, source, searchImage);

			_PERFORMANCE_MONITOR.Start();

			foreach (var searchArea in result.SearchAreas)
			{
				Logger.Log($"Calculating matches for search area: {searchArea}");

				result.Matches.AddRange(CollectMatches(source, searchImage, searchRectangle.RelativeClickPoint, similiarityThreshold, condition.SearchRectangleName, condition.TemplateName, searchArea));
			}

			if (!condition.DisableSearchAreaFallback && result.Matches.Count < condition.MinimumOccurrence && !result.SearchAreas.Contains(SearchAreaFactory.FromEnum(NamedSearchArea.Full)))
			{
				Logger.Log($"Search in given search areas not resulted minimum {condition.MinimumOccurrence} matches. Searching in full image!");

				result.Matches = CollectMatches(source, searchImage, searchRectangle.RelativeClickPoint, similiarityThreshold, condition.SearchRectangleName, condition.TemplateName);
			}

			_PERFORMANCE_MONITOR.Stop();

			Logger.Log($"Mathes calculated. Full search time: {_PERFORMANCE_MONITOR.Elapsed}");

			Logger.GraphicalLog(result, condition.TemplateName, condition.SearchRectangleName);

			return result;
		}

		private static List<ImageMatch> CollectMatches(Bitmap source, Bitmap searchImage, SerializableSize relativeClickPoint, float similiarityThreshold, string searchRectangleName, string templateName, SerializableRectangle area = null)
		{
			using var log = Logger.LogBlock();

			MouseSafetyMeasures.CheckForIntentionalEmergencyStop();

			ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(similiarityThreshold);

			Logger.Log($"Processing image, searching for {searchRectangleName} of {templateName}");

			var findResult = area is null ? matching.ProcessImage(source, searchImage) : matching.ProcessImage(source, searchImage, (Rectangle)area);

			var result = findResult.Select(tm =>
				new ImageMatch((SerializablePoint)tm.Rectangle.Location + relativeClickPoint, (SerializableRectangle)tm.Rectangle)).ToList();

			result.ForEach(m => Logger.Log($"Match found for {searchRectangleName} of {templateName}{(area is null ? "" : $" in {area} search area") } at {m.ClickPoint}. At search time: {_PERFORMANCE_MONITOR.Elapse}"));

			return result;
		}

		private static Bitmap CropTemplateImage(Bitmap bitmap, Rectangle rectangle)
		{
			using var log = Logger.LogBlock();

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
