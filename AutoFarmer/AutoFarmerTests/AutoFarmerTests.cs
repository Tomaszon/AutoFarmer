using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
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

			WorkflowGraph graph = WorkflowGraph.FromConfig();

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
	}
}
