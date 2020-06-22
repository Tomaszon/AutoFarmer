using System.Drawing;

namespace AutoFarmer.Models
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

		public override string ToString()
		{
			return $"X: {X}, Y: {Y}";
		}

		public static Size operator -(SerializablePoint point, SerializablePoint other)
		{
			return new Size(point.X - other.X, point.Y - other.Y);
		}

		public static implicit operator Point(SerializablePoint point)
		{
			return new Point(point.X, point.Y);
		}

		public static implicit operator SerializablePoint(Point point)
		{
			return new SerializablePoint(point.X, point.Y);
		}
	}
}