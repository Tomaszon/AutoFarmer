using AutoFarmer.Models;
using AutoFarmer.Models.GraphNamespace;
using AutoFarmer.Models.ImageMatching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace AutoFarmerTests
{
	[TestClass]
	public class AutoFarmerTests
	{
		[TestMethod]
		public void TestMethod2v1()
		{
			TestMethod2Core(0.99f, "TestTemplate2v1", "TestRectangle2v1", NamedSearchArea.Right);
		}

		[TestMethod]
		public void TestMethod2v2()
		{
			TestMethod2Core(0.99f, "TestTemplate2v2", "TestRectangle2v2", NamedSearchArea.Left);
		}

		[TestMethod]
		public void TestMethod2v5()
		{
			TestMethod2Core(0.99f, "TestTemplate2v5", "TestRectangle2v5");
		}

		private void TestMethod2Core(float similiarityThreshold, string templateName, string searchRectangleName, params NamedSearchArea[] namedSearchAreas)
		{
			Config.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\configs\config.json");

			Dictionary<MatchOrderBy, MatchOrderLike> o = new Dictionary<MatchOrderBy, MatchOrderLike>() { { MatchOrderBy.Y, MatchOrderLike.Descending } };

			MatchCondition c = new MatchCondition() { SearchRectangleName = searchRectangleName, TemplateName = templateName, MaximumOccurrence = 4, OrderBy = o };

			var sr = new SearchRectangle() { X = 1323, Y = 382, W = 130, H = 20, NamedSearchAreas = namedSearchAreas };

			sr.Init();

			var srs = new Dictionary<string, SearchRectangle> { { searchRectangleName, sr } };

			ImageMatchFinder.Instance = JsonConvert.DeserializeObject<ImageMatchFinder>(File.ReadAllText(Config.Instance.ImageMatchFinderConfigPath));

			var t = new ImageMatchTemplate() { Name = templateName, Bitmap = ImageFactory.ConvertBitmap(Properties.Resources.assignmentsCompleted), SearchRectangles = srs };
			var ts = new List<ImageMatchTemplate> { t };

			ImageMatchFinder.Instance.Templates = ts;

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var points = ImageMatchFinder.FindClickPointForTemplate(c, ImageFactory.ConvertBitmap(Properties.Resources.assignmentsCompleted), similiarityThreshold);
			stopwatch.Stop();

			Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");

			var expectedPoint1 = new SerializablePoint() { X = 1388, Y = 392 };
			var expectedPoint2 = new SerializablePoint() { X = 1388, Y = 526 };

			Assert.AreEqual(2, points.Count);
			Assert.AreEqual(expectedPoint1.X, points[1].X);
			Assert.AreEqual(expectedPoint1.Y, points[1].Y);
			Assert.AreEqual(expectedPoint2.X, points[0].X);
			Assert.AreEqual(expectedPoint2.Y, points[0].Y);
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

		[TestMethod]
		public void TestMethod7()
		{
			var customSearchAreas = new[]
			{
				new SerializableRectangle()
				{
					X = 898,
					Y = 0,
					H = 1080,
					W = 1022
				},
				new SerializableRectangle()
				{
					X = 850,
					Y = 100,
					W = 500,
					H = 500
				}
			};

			var sr = new SearchRectangle() { X = 1322, Y = 383, W = 132, H = 20, SearchAreas = new List<SerializableRectangle>(customSearchAreas) };

			Assert.ThrowsException<AutoFarmerException>(() => sr.Init());
		}
	}
}
