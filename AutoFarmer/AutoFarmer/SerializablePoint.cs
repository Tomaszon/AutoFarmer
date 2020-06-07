using System.Drawing;

namespace image_search_test
{
	public class SerializablePoint
	{
		public int X { get; set; }

		public int Y { get; set; }

		public SerializablePoint(int x, int y)
		{
			X = x;
			Y = y;
		}

		//public static implicit operator Point(SerializablePoint point)
		//{
		//	return new Point(point.X, point.Y);
		//}
	}
}