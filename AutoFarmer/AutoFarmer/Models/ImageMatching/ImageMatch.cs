using System.Collections.Generic;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatch
	{
		public SerializableRectangle MatchRectangle { get; set; }

		public SerializableRectangle ScaledMatchRectangle { get; set; }

		public SerializablePoint ClickPoint { get; set; }

		public SerializablePoint ScaledClickPoint { get; set; }

		public ImageMatch(SerializablePoint scaledClickPoint, SerializableRectangle scaledMatchRectangle, double scale)
		{
			ScaledClickPoint = scaledClickPoint;
			ScaledMatchRectangle = scaledMatchRectangle;
			ClickPoint = scaledClickPoint.Scale(scale);
			MatchRectangle = scaledMatchRectangle.Scale(scale);
		}
	}
}
