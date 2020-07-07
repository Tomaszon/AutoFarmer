using System.Drawing;

namespace AutoFarmer.Models.Common
{
	public class SerializableSize
	{
		public int W { get; set; }

		public int H { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is SerializableSize p)
			{
				return W == p.W && H == p.H;
			}

			return false;
		}

		public override string ToString()
		{
			return ((Size)this).ToString();
		}

		public static explicit operator SerializableSize(Size size)
		{
			return new SerializableSize()
			{
				W = size.Width,
				H = size.Height
			};
		}

		public static explicit operator Size(SerializableSize size)
		{
			return new Size(size.W, size.H);
		}

		public static explicit operator SerializableSize(SerializablePoint point)
		{
			return new SerializableSize()
			{
				W = point.X,
				H = point.Y
			};
		}
	}
}