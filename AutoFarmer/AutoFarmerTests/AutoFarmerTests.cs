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

			MatchCondition c = new MatchCondition() { SearchRectangleName = "TestRectangle1", TemplateName = "TestTemplate1" };

			var sr = new SearchRectangle() { X = 210, Y = 712, W = 28, H = 14 };
			sr.Init();

			var srs = new Dictionary<string, SearchRectangle> { { "TestRectangle1", sr } };

			var t = new ImageMatchTemplate() { Name = "TestTemplate1", Bitmap = Properties.Resources.characterSelector1, SearchRectangles = srs };

			var ts = new List<ImageMatchTemplate> { t };

			ImageMatchFinder.Instance = JsonConvert.DeserializeObject<ImageMatchFinder>(File.ReadAllText(Config.Instance.ImageMatchFinderConfigPath));
			ImageMatchFinder.Instance.Templates = ts;

			var points = ImageMatchFinder.FindClickPointForTemplate(c, Properties.Resources.characterSelector1Source, 0.99f);

			var expectedPoint = new SerializablePoint() { X = 480, Y = 843 };

			Assert.AreEqual(expectedPoint, points[0]);
		}

		[TestMethod]
		public void TestMethod2v1()
		{
			TestMethod2Core("TestTemplate2v1", "TestRectangle2v1", AutoSearchAreaMode.Full);
		}

		[TestMethod]
		public void TestMethod2v2()
		{
			TestMethod2Core("TestTemplate2v2", "TestRectangle2v2", AutoSearchAreaMode.LeftRight);
		}

		[TestMethod]
		public void TestMethod2v3()
		{
			TestMethod2Core("TestTemplate2v3", "TestRectangle2v3", AutoSearchAreaMode.UpperLower);
		}

		[TestMethod]
		public void TestMethod2v4()
		{
			TestMethod2Core("TestTemplate2v4", "TestRectangle2v4", AutoSearchAreaMode.Quarter);
		}

		[TestMethod]
		public void TestMethod2v5()
		{
			TestMethod2Core("TestTemplate2v5", "TestRectangle2v5", customSearchAreas: new SerializableRectangle() { X = 898, Y = 0, H = 1080, W = 1022 });
		}

		private void TestMethod2Core(string templateName, string searchRectangleName, AutoSearchAreaMode autoSearchAreaMode = default, params SerializableRectangle[] customSearchAreas)
		{
			Config.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\configs\config.json");

			Dictionary<MatchOrderBy, MatchOrderLike> o = new Dictionary<MatchOrderBy, MatchOrderLike>() { { MatchOrderBy.Y, MatchOrderLike.Descending } };

			MatchCondition c = new MatchCondition() { SearchRectangleName = searchRectangleName, TemplateName = templateName, MaximumOccurrence = 2, OrderBy = o };

			var sr = new SearchRectangle() { X = 1322, Y = 383, W = 132, H = 20, AutoSearchAreaMode = autoSearchAreaMode, SearchAreas = new List<SerializableRectangle>(customSearchAreas) };
			sr.Init();

			var srs = new Dictionary<string, SearchRectangle> { { searchRectangleName, sr } };

			var t = new ImageMatchTemplate() { Name = templateName, Bitmap = Properties.Resources.assignmentsCompleted, SearchRectangles = srs };

			var ts = new List<ImageMatchTemplate> { t };

			ImageMatchFinder.Instance = JsonConvert.DeserializeObject<ImageMatchFinder>(File.ReadAllText(Config.Instance.ImageMatchFinderConfigPath));
			ImageMatchFinder.Instance.Templates = ts;

			var points = ImageMatchFinder.FindClickPointForTemplate(c, Properties.Resources.assignmentsCompleted, 0.99f);

			var expectedPoint1 = new SerializablePoint() { X = 1388, Y = 393 };
			var expectedPoint2 = new SerializablePoint() { X = 1388, Y = 527 };

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

		[TestMethod]
		public void TestMethod5()
		{
			SerializablePoint p1 = new SerializablePoint() { X = 10, Y = 20 };
			SerializablePoint p2 = new SerializablePoint() { X = 10, Y = 20 };
			SerializablePoint p3 = new SerializablePoint() { X = 5, Y = 20 };
			SerializablePoint p4 = null;

			Assert.AreEqual(p1, p2);
			Assert.AreNotEqual(p1, p3);
			Assert.AreNotEqual(p1, p4);
		}

		[TestMethod]
		public void TestMethod6()
		{
			var e = (int)NamedSearchArea.Left;

			Assert.AreEqual(0b1101, e | 0b0000);
			Assert.AreEqual(0b1101, e & 0b1111);
			Assert.AreEqual(0b1100, e & 0b1100);
		}
	}
}
