using System.Drawing;

namespace AutoFarmer.Models
{
	public class SerializableRectangle
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int W { get; set; }

		public int H { get; set; }

		public static implicit operator Rectangle(SerializableRectangle rec)
		{
			return new Rectangle(rec.X, rec.Y, rec.W, rec.H);
		}
	}
}