using AutoFarmer.Models;
using AutoFarmer.Models.GraphNamespace;
using AutoFarmer.Models.ImageMatching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AutoFarmerTests
{
	[TestClass]
	public class AutoFarmerTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			Config.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\configs\config.json");

			MatchCondition c = new MatchCondition() { SearchRectangleName = "TestRectangle", TemplateName = "TestTemplate" };

			var sr = new SearchRectangle() { X = 210, Y = 712, W = 28, H = 14 };

			var srs = new Dictionary<string, SearchRectangle>
			{
				{ "TestRectangle", sr }
			};

			var t = new ImageMatchTemplate() { Name = "TestTemplate", Bitmap = Properties.Resources.characterSelector1, SearchRectangles = srs };

			var ts = new List<ImageMatchTemplate>
			{
				t
			};

			ImageMatchFinder.Instance = JsonConvert.DeserializeObject<ImageMatchFinder>(File.ReadAllText(Config.Instance.ImageMatchFinderConfigPath));
			ImageMatchFinder.Instance.Templates = ts;

			var points = ImageMatchFinder.FindClickPointForTemplate(c, Properties.Resources.characterSelector1Source, 0.99f);

			var expectedPoint = new Point(480, 843);

			Assert.AreEqual(expectedPoint, points[0]);
		}

		[TestMethod]
		public void TestMethod2()
		{
			Config.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\configs\config.json");

			Dictionary<MatchOrderBy, MatchOrderLike> o = new Dictionary<MatchOrderBy, MatchOrderLike>()
			{
				{ MatchOrderBy.Y, MatchOrderLike.Descending }
			};

			MatchCondition c = new MatchCondition() { SearchRectangleName = "TestRectangle", TemplateName = "TestTemplate", MaximumOccurrence = 2, OrderBy = o };

			var sr = new SearchRectangle() { X = 1322, Y = 383, W = 132, H = 20 };

			var srs = new Dictionary<string, SearchRectangle>
			{
				{ "TestRectangle", sr }
			};

			var t = new ImageMatchTemplate() { Name = "TestTemplate", Bitmap = Properties.Resources.assignmentsCompleted, SearchRectangles = srs };

			var ts = new List<ImageMatchTemplate>
			{
				t
			};

			ImageMatchFinder.Instance = JsonConvert.DeserializeObject<ImageMatchFinder>(File.ReadAllText(Config.Instance.ImageMatchFinderConfigPath));
			ImageMatchFinder.Instance.Templates = ts;

			var points = ImageMatchFinder.FindClickPointForTemplate(c, Properties.Resources.assignmentsCompleted, 0.99f);

			var expectedPoint1 = new Point(1388, 393);
			var expectedPoint2 = new Point(1388, 527);

			Assert.AreEqual(expectedPoint1, points[1]);
			Assert.AreEqual(expectedPoint2, points[0]);
		}

		[TestMethod]
		public void TestMethod3()
		{
			List<ActionNode> a = ActionNodeOptions.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\Configs\actionNodes\startScrollUpButtons.json");

			Assert.AreEqual(a.Count, 10);

			for (int i = 0; i < a.Count; i++)
			{
				Assert.AreEqual($"startScrollUpButton0{i}", a[i].Name);
			}
		}

		[TestMethod]
		public void TestMethod4()
		{
			List<ConditionEdge> a = ConditionEdgeOptions.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\Configs\conditionEdges\startScrollButtons.json");

			Assert.AreEqual(a.Count, 20);

			Assert.AreEqual($"startScrollDownButton05", a[5].StartNodeName);

			Assert.AreEqual($"startScrollUpButton06", a[16].StartNodeName);
		}
	}
}
