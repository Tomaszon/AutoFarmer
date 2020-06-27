using System.Drawing;

namespace AutoFarmer.Models.Common
{
	public class SerializableRectangle
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int W { get; set; }

		public int H { get; set; }

		public override string ToString()
		{
			return ((Rectangle)this).ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is SerializableRectangle r)
			{
				return X == r.X && Y == r.Y && W == r.W && H == r.H;
			}

			return false;
		}

		public static explicit operator Rectangle(SerializableRectangle rec)
		{
			return new Rectangle(rec.X, rec.Y, rec.W, rec.H);
		}

		public static explicit operator SerializableRectangle(Rectangle rec)
		{
			return new SerializableRectangle()
			{
				X = rec.X,
				Y = rec.Y,
				W = rec.Width,
				H = rec.Height
			};
		}
	}
}