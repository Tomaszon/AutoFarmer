using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Models.Graph.ActionNodes;
using AutoFarmer.Models.Graph.ConditionEdges;
using AutoFarmer.Models.ImageMatching;
using AutoFarmer.Services;
using AutoFarmer.Services.Imaging;
using AutoFarmer.Services.InputHandling;
using AutoFarmer.Services.Logging;
using AutoFarmer.Services.ReportBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoFarmerTests
{
	[TestClass]
	public class AutoFarmerTests
	{
		private string UserName => System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];

		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out Point lpPoint);

		[TestMethod]
		public void TestMethod2v1()
		{
			TestMethod2(
				0.99f,
				"assignments",
				new[] { "collectButton" },
				"completedTab-collectButtons",
				NamedSearchArea.Top,
				new SerializablePoint() { X = 76, Y = 169 });
		}

		[TestMethod]
		public void TestMethod2v2()
		{
			TestMethod2(
				0.99f,
				"assignments",
				new[] { "collectButton" },
				"completedTab-collectButtons",
				NamedSearchArea.Full,
				new SerializablePoint() { X = 76, Y = 169 });
		}

		private void TestMethod2(float similiarityThreshold, string templateName, string[] searchRectangleNames, string conditionEdgeName, NamedSearchArea namedSearchArea, params SerializablePoint[] expectedPoints)
		{
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\configs\config.json");

			Config.Instance.ScreenSize = new SerializableSize() { W = 200, H = 500 };

			Logger.FromConfig();

			MouseSafetyMeasures.FromConfig();
			MouseSafetyMeasures.Instance.IsEnabled = false;

			ImageMatchFinder.FromConfig();
			foreach (var srName in searchRectangleNames)
			{
				var sr = ImageMatchFinder.Instance.Templates.Single(t => t.Name == templateName).SearchRectangles.Single(t => t.Key == srName).Value;
				sr.SearchAreas = new List<SerializableRectangle>(new[] { SearchAreaFactory.FromEnum(namedSearchArea) });
			}

			ActionGraph graph = ActionGraph.FromConfig();

			var condition = graph.ConditionEdges.Single(e => e.Name == conditionEdgeName).Condition;

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
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\configs\config.json");

			Logger.FromConfig();

			List<ActionNode> a = ActionNodeOptions.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\Configs\Packages\STO\actionNodes\startScrollUpButtons.json");

			Assert.AreEqual(a.Count, 10);

			for (int i = 0; i < a.Count; i++)
			{
				Assert.AreEqual($"startScrollUpButton0{i}", a[i].Name);
			}
		}

		[TestMethod]
		public void TestMethod4()
		{
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\configs\config.json");

			Logger.FromConfig();

			List<ConditionEdge> a = ConditionEdgeOptions.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\Configs\Packages\STO\conditionEdges\startScrollButtons.json");

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
		public void TestMethod8()
		{
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\configs\config.json");

			Logger.FromConfig();

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
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\configs\config.json");

			Logger.FromConfig();

			Test9(NamedSearchArea.Bottom, 0, 540, 1920, 540, out var a1, out var refA1);
			Test9(NamedSearchArea.BottomLeft, 0, 540, 960, 540, out var a2, out var refA2);
			Test9(NamedSearchArea.BottomRight, 960, 540, 960, 540, out var a3, out var refA3);
			Test9(NamedSearchArea.Full, 0, 0, 1920, 1080, out var a4, out var refA4);
			Test9(NamedSearchArea.Left, 0, 0, 960, 1080, out var a5, out var refA5);
			Test9(NamedSearchArea.Middle, 480, 270, 960, 540, out var a6, out var refA6);
			Test9(NamedSearchArea.MiddleHalfHeight, 0, 270, 1920, 540, out var a7, out var refA7);
			Test9(NamedSearchArea.MiddleHalfWidth, 480, 0, 960, 1080, out var a8, out var refA8);
			Test9(NamedSearchArea.Right, 960, 0, 960, 1080, out var a9, out var refA9);
			Test9(NamedSearchArea.Top, 0, 0, 1920, 540, out var a10, out var refA10);
			Test9(NamedSearchArea.TopLeft, 0, 0, 960, 540, out var a11, out var refA11);
			Test9(NamedSearchArea.TopRight, 960, 0, 960, 540, out var a12, out var refA12);

			var areas = new[] { a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12 };
			var refAreas = new[] { refA1, refA2, refA3, refA4, refA5, refA6, refA7, refA8, refA9, refA10, refA11, refA12 };

			for (int i = 0; i < refAreas.Length; i++)
			{
				Assert.AreEqual(refAreas[i], areas[i]);
			}
		}

		private void Test9(NamedSearchArea area, int refX, int refY, int refW, int refH, out SerializableRectangle a, out SerializableRectangle refA)
		{
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\configs\config.json");

			Logger.FromConfig();

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

		[TestMethod]
		public void TestMethod10()
		{
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\configs\config.json");

			Logger.FromConfig();

			ReportBuilder.FromJsonFileWithConfig(Path.Combine(Config.Instance.ConfigDirectory, "reportBuilderConfig.json"));

			ReportBuilder.Add("key1", "msg1", ReportMessageType.Success);
			ReportBuilder.Add("key1", "msg2", ReportMessageType.Success);

			ReportBuilder.Add("key2", "msg3", ReportMessageType.Success);

			ReportBuilder.Add("key3", "msg4", ReportMessageType.Fail);

			ReportBuilder.Commit(ReportMessageType.Fail);

			ReportBuilder.Generate();
		}

		[TestMethod]
		public void TestMethod11()
		{
			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\Configs\config.json");

			Logger.FromConfig();

			MouseSafetyMeasures.FromConfig();
			MouseSafetyMeasures.Instance.IsEnabled = false;

			InputSimulator.FromConfig();

			InputSimulator.Simulate(new[] { "Move:100,100" }, null);
		}

		[TestMethod]
		public void TestMethod12()
		{
			Thread.Sleep(2500);

			Config.FromJsonFile(@$"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\Configs\config.json");

			Logger.FromConfig();

			MouseSafetyMeasures.FromConfig();
			MouseSafetyMeasures.Instance.IsEnabled = false;

			InputSimulator.FromConfig();

			GlobalStateStorage.FromConfig();

			//InputSimulator.Simulate(new[] { "Multiply:24,transactionMultiplier" }, new SerializablePoint() { X = 960, Y = 540 });
			//InputSimulator.Simulate(new[] { "SPACE" }, new SerializablePoint() { X = 960, Y = 540 });
			//InputSimulator.Simulate(new[] { "Multiply:24,transactionMultiplier" }, new SerializablePoint() { X = 960, Y = 540 });
		}
	}
}
