using System.Drawing;

namespace AutoFarmer.Models.Common
{
	public class SerializablePoint
	{
		public int X { get; set; }

		public int Y { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is SerializablePoint p)
			{
				return X == p.X && Y == p.Y;
			}

			return false;
		}

		public override string ToString()
		{
			return ((Point)this).ToString();
		}

		public static SerializableSize operator -(SerializablePoint point, SerializablePoint other)
		{
			return new SerializableSize()
			{
				W = point.X - other.X,
				H = point.Y - other.Y
			};
		}

		public static SerializablePoint operator +(SerializablePoint point, SerializableSize other)
		{
			return new SerializablePoint()
			{
				X = point.X + other.W,
				Y = point.Y + other.H
			};
		}

		public static explicit operator Point(SerializablePoint point)
		{
			return new Point(point.X, point.Y);
		}

		public static explicit operator SerializablePoint(Point point)
		{
			return new SerializablePoint()
			{
				X = point.X,
				Y = point.Y
			};
		}

		public static explicit operator SerializablePoint(SerializableSize size)
		{
			return new SerializablePoint()
			{
				X = size.W,
				Y = size.H
			};
		}
	}
}