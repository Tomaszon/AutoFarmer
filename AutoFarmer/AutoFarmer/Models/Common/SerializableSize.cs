using System.Drawing;

namespace AutoFarmer.Models
{
	public class SerializableSize
	{
		public int W { get; set; }

		public int H { get; set; }

		public static explicit operator Size(SerializableSize size)
		{
			return new Size(size.W, size.H);
		}

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

		public SerializableSize Scale(double scale)
		{
			return new SerializableSize()
			{
				H = (int)(H * scale),
				W = (int)(W * scale)
			};
		}
	}
}