using AutoFarmer.Models.Common;
using System.Collections.Generic;

namespace AutoFarmer.Models.ImageMatching
{
	public class SearchRectangleOptions : SearchRectangleBase
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int W { get; set; }

		public int H { get; set; }

		public List<NamedSearchArea>? NamedSearchAreas { get; set; }

		public List<SerializableRectangle>? SearchAreas { get; set; }

		public SearchRectangle ToSearchRectangle()
		{
			return new SearchRectangle(ClickPoint, SearchAreas, NamedSearchAreas, X, Y, W, H);
		}
	}
}