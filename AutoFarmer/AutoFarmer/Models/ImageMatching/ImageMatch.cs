using System.Collections.Generic;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatch
	{
		public SerializableRectangle MatchRectangle { get; set; }

		public SerializablePoint ClickPoint { get; set; }

		public ImageMatch(SerializablePoint clickPoint, SerializableRectangle matchRectangle)
		{
			ClickPoint = clickPoint;
			MatchRectangle = matchRectangle;
		}
	}
}
