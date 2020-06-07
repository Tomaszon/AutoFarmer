using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;

namespace image_search_test
{
	internal class Program
	{
		#region test
		private static void Main(string[] args)
		{
			string content = File.ReadAllText(@".\configs\ImageMatchFinderConfig.Json");
			ImageMatchFinder imf = JsonConvert.DeserializeObject<ImageMatchFinder>(content);

			var testSource = (Bitmap)Image.FromFile(@"C:\users\toti9\downloads\testsource.png");

			var testSourceOutput = (Bitmap)testSource.Clone();

			var testTemplate = imf.Templates["AssignmentDetails"];
			Point p = imf.FindClickPointForTemplate(testSource, testTemplate.Bitmap, testTemplate.SearchRectangles["BeginAssignment"]);

			HighlightFind(testSourceOutput, p);

			testSourceOutput.Save(@"C:\Users\toti9\Downloads\testSourceOutput.png");

			Console.WriteLine(p);
			Console.WriteLine("Done");
			Console.ReadKey();
		}

		private static void HighlightFind(Bitmap bitmap, Point point)
		{
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.DrawRectangle(Pens.Red, new Rectangle(point.X - 1, point.Y - 1, 3, 3));
			}
		}
		#endregion test


	}
}
