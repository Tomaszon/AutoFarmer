using AutoFarmer.Models.Common;
using System.Collections.Generic;
using System.Drawing;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchResult
	{
		public List<ImageMatch> Matches { get; set; } = new List<ImageMatch>();

		public List<SerializableRectangle> SearchAreas { get; set; }

		public Bitmap Source { get; set; }

		public Bitmap SearchImage { get; set; }

		public ImageMatchResult(List<SerializableRectangle> searchAreas, Bitmap source, Bitmap searchImage)
		{
			SearchAreas = searchAreas;
			Source = source;
			SearchImage = searchImage;
		}
	}
}
