using System.Drawing;

namespace AutoFarmer.Models.Common
{
	public class SerializableRectangle
	{
		public SerializablePoint Position { get; set; } = new SerializablePoint();

		public SerializableSize Size { get; set; } = new SerializableSize();

		public override string ToString()
		{
			return ((Rectangle)this).ToString();
		}

		public override bool Equals(object? obj)
		{
			if (obj is SerializableRectangle r)
			{
				return Position.Equals(r.Position) && Size.Equals(r.Size);
			}

			return false;
		}

		public static explicit operator Rectangle(SerializableRectangle rec)
		{
			return new Rectangle(rec.Position.X, rec.Position.Y, rec.Size.W, rec.Size.H);
		}

		public static explicit operator SerializableRectangle(Rectangle rec)
		{
			return new SerializableRectangle()
			{
				Position = new SerializablePoint()
				{
					X = rec.X,
					Y = rec.Y
				},
				Size = new SerializableSize()
				{
					W = rec.Width,
					H = rec.Height
				}
			};
		}
	}
}