using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Models.ImageMatching;
using AutoFarmer.Models.InputHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AutoFarmerTests
{
	[TestClass]
	public class AutoFarmerTests
	{
		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out Point lpPoint);

		[TestMethod]
		public void TestMethod2v1()
		{
			TestMethod2Core(
				0.99f,
				"assignmentsCompleted",
				new[] { "collectButton" },
				"completedTab-collectButtons",
				NamedSearchArea.Right,
				new SerializablePoint() { X = 1388, Y = 527 },
				new SerializablePoint() { X = 1388, Y = 393 });
		}

		[TestMethod]
		public void TestMethod2v2()
		{
			TestMethod2Core(
				0.99f,
				"assignmentsCompleted",
				new[] { "collectButton" },
				"completedTab-collectButtons",
				NamedSearchArea.Left,
				new SerializablePoint() { X = 1388, Y = 527 },
				new SerializablePoint() { X = 1388, Y = 393 });
		}

		[TestMethod]
		public void TestMethod2v3()
		{
			TestMethod2Core(
				0.99f,
				"assignmentsCompleted",
				new[] { "collectButton" },
				"completedTab-collectButtons",
				NamedSearchArea.Full,
				new SerializablePoint() { X = 1388, Y = 527 },
				new SerializablePoint() { X = 1388, Y = 393 });
		}

		private void TestMethod2Core(float similiarityThreshold, string templateName, string[] searchRectangleNames, string conditionEdgeName, NamedSearchArea namedSearchArea, params SerializablePoint[] expectedPoints)
		{
			Config.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\configs\config.json");

			MouseSafetyMeasures.FromConfig();
			MouseSafetyMeasures.Instance.IsEnabled = false;

			ImageMatchFinder.FromConfig();
			foreach (var srName in searchRectangleNames)
			{
				var sr = ImageMatchFinder.Instance.Templates.Single(t => t.Name == templateName).SearchRectangles.Single(t => t.Key == srName).Value;
				sr.NamedSearchAreas = new[] { namedSearchArea };
				sr.SearchAreas.Clear();
				sr.Init();
			}

			ActionGraph graph = ActionGraph.FromConfig();

			var condition = graph.ConditionEdges.Single(e => e.Name == conditionEdgeName).Condition;
			condition.MinimumOccurrence = 2;

			Bitmap sourceImage = (Bitmap)Image.FromFile(Path.Combine(Config.Instance.ImageMatchTemplateResourcesDirectory, $"{templateName}.png"));

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var points = ImageMatchFinder.FindClickPointForTemplate(condition, ImageFactory.ConvertBitmap(sourceImage), similiarityThreshold);
			stopwatch.Stop();

			Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");

			Assert.AreEqual(expectedPoints.Length, points.Count);

			for (int i = 0; i < expectedPoints.Length; i++)
			{
				Assert.AreEqual(expectedPoints[i].X, points[i].X, 2);
				Assert.AreEqual(expectedPoints[i].Y, points[i].Y, 2);
			}
		}

		[TestMethod]
		public void TestMethod3()
		{
			List<ActionNode> a = ActionNodeOptions.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\Configs\Packages\STO\actionNodes\startScrollUpButtons.json");

			Assert.AreEqual(a.Count, 10);

			for (int i = 0; i < a.Count; i++)
			{
				Assert.AreEqual($"startScrollUpButton0{i}", a[i].Name);
			}
		}

		[TestMethod]
		public void TestMethod4()
		{
			List<ConditionEdge> a = ConditionEdgeOptions.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\Configs\Packages\STO\conditionEdges\startScrollButtons.json");

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
		public void TestMethod7()
		{
			var customSearchAreas = new[]
			{
				new SerializableRectangle()
				{
					Position = new SerializablePoint()
					{
						X = 898,
						Y = 0
					},
					Size = new SerializableSize()
					{
						H = 1080,
						W = 1022
					}
				},
				new SerializableRectangle()
				{
					Position = new SerializablePoint()
					{
						X = 850,
						Y = 100
					},
					Size = new SerializableSize()
					{
						W = 500,
						H = 500
					}
				}
			};

			var sr = new SearchRectangle()
			{
				Rectangle = new SerializableRectangle()
				{
					Position = new SerializablePoint()
					{
						X = 1322,
						Y = 383
					},
					Size = new SerializableSize()
					{
						W = 132,
						H = 20
					}
				},
				SearchAreas = new List<SerializableRectangle>(customSearchAreas)
			};

			Assert.ThrowsException<AutoFarmerException>(() => sr.Init());
		}

		[TestMethod]
		public void TestMethod8()
		{
			Config.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\configs\config.json");

			MouseSafetyMeasures.FromConfig();
			MouseSafetyMeasures.Instance.IsEnabled = false;

			InputSimulator.FromConfig();

			InputSimulator.Simulate(new[] { "Move:960,540" }, null);

			GetCursorPos(out var refP);

			Assert.AreEqual(refP, new Point(960, 540));
			Assert.ThrowsException<AutoFarmerException>(() => InputSimulator.Simulate(new[] { "Move:12asd3,321" }, null));
			Assert.ThrowsException<AutoFarmerException>(() => InputSimulator.Simulate(new[] { "Alma:123,321" }, null));
		}

		[TestMethod]
		public void TestMethod9()
		{
			Config.FromJsonFile(@"C:\Users\toti9\Documents\GitHub\AutoFarmer\AutoFarmer\AutoFarmer\configs\config.json");

			Test9Core(NamedSearchArea.Bottom, 0, 540, 1920, 540, out var a1, out var refA1);
			Test9Core(NamedSearchArea.BottomLeft, 0, 540, 960, 540, out var a2, out var refA2);
			Test9Core(NamedSearchArea.BottomRight, 960, 540, 960, 540, out var a3, out var refA3);
			Test9Core(NamedSearchArea.Full, 0, 0, 1920, 1080, out var a4, out var refA4);
			Test9Core(NamedSearchArea.Left, 0, 0, 960, 1080, out var a5, out var refA5);
			Test9Core(NamedSearchArea.Middle, 480, 270, 960, 540, out var a6, out var refA6);
			Test9Core(NamedSearchArea.MiddleHalfHeight, 0, 270, 1920, 540, out var a7, out var refA7);
			Test9Core(NamedSearchArea.MiddleHalfWidth, 480, 0, 960, 1080, out var a8, out var refA8);
			Test9Core(NamedSearchArea.Right, 960, 0, 960, 1080, out var a9, out var refA9);
			Test9Core(NamedSearchArea.Top, 0, 0, 1920, 540, out var a10, out var refA10);
			Test9Core(NamedSearchArea.TopLeft, 0, 0, 960, 540, out var a11, out var refA11);
			Test9Core(NamedSearchArea.TopRight, 960, 0, 960, 540, out var a12, out var refA12);

			var areas = new[] { a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12 };
			var refAreas = new[] { refA1, refA2, refA3, refA4, refA5, refA6, refA7, refA8, refA9, refA10, refA11, refA12 };

			for (int i = 0; i < refAreas.Length; i++)
			{
				Assert.AreEqual(refAreas[i], areas[i]);
			}
		}

		private void Test9Core(NamedSearchArea area, int refX, int refY, int refW, int refH, out SerializableRectangle a, out SerializableRectangle refA)
		{
			a = SearchAreaFactory.FromEnum(area);

			refA = new SerializableRectangle()
			{
				Position = new SerializablePoint()
				{
					X = refX,
					Y = refY
				},
				Size = new SerializableSize()
				{
					W = refW,
					H = refH
				}
			};
		}
	}
}
