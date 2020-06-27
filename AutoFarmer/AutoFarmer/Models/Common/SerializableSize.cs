using System.Drawing;

namespace AutoFarmer.Models.Common
{
	public class SerializableSize
	{
		public int W { get; set; }

		public int H { get; set; }

		public override string ToString()
		{
			return ((Size)this).ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is SerializableSize p)
			{
				return W == p.W && H == p.H;
			}

			return false;
		}

		public static explicit operator Size(SerializableSize size)
		{
			return new Size(size.W, size.H);
		}
	}
}