﻿using System.Collections.Generic;
using System.Drawing;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchResult
	{
		public List<ImageMatch> Matches { get; set; } = new List<ImageMatch>();

		public List<SerializableRectangle> SearchAreas { get; set; } = new List<SerializableRectangle>();

		public List<SerializableRectangle> ScaledSearchAreas { get; set; } = new List<SerializableRectangle>();

		public Bitmap ScaledSource { get; set; }

		public Bitmap ScaledSearchImage { get; set; }
	}
}