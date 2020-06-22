using System.Drawing;

namespace AutoFarmer.Models.ImageMatching
{
	public class SearchRectangle
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int W { get; set; }

		public int H { get; set; }

		public SerializablePoint ClickPoint { get; set; }

		public SerializableRectangle SearchArea { get; set; }

		public NamedSearchArea[] NamedSearchAreas { get; set; }

		public AutoSearchAreaMode AutoSearchAreaMode { get; set; } = AutoSearchAreaMode.LeftRight;

		public Size RelativeClickPoint
		{
			get
			{
				return ClickPoint == null ? new Size(W / 2, H / 2) : new Size(ClickPoint.X - X, ClickPoint.Y - Y);
			}
		}
	}
}