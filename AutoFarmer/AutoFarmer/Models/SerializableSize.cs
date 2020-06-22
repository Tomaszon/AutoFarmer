using System.Drawing;

namespace AutoFarmer.Models
{
	public class SerializableSize
	{
		public int W { get; set; }

		public int H { get; set; }

		public static implicit operator Size(SerializableSize size)
		{
			return new Size(size.W, size.H);
		}
	}
}