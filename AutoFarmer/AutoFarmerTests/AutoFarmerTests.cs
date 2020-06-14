using AutoFarmer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

			ImageFindCondition c = new ImageFindCondition() { SearchRectangleName = "TestRectangle", TemplateName = "TestTemplate" };

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

			var imf = new ImageMatchFinder() { Scale = 0.5, Templates = ts };

			var points = imf.FindClickPointForTemplate(Properties.Resources.characterSelector1Source, c, 0.99f);

			var expectedPoint = new Point(480, 843);

			Assert.AreEqual(expectedPoint, points[0]);
		}
	}
}
