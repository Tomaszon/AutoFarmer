using AutoFarmer.Models.Common;
using System.Collections.Generic;

namespace AutoFarmer.Models.ImageMatching
{
	public class SearchRectangleOptions
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int W { get; set; }

		public int H { get; set; }

		public SerializablePoint ClickPoint { get; set; }

		public List<SerializableRectangle> SearchAreas { get; set; } = new List<SerializableRectangle>();

		public NamedSearchArea[] NamedSearchAreas { get; set; }

		public SearchRectangle ToSearchRectangle()
		{
			return new SearchRectangle()
			{
				ClickPoint = ClickPoint,
				NamedSearchAreas = NamedSearchAreas,
				SearchAreas = SearchAreas,
				Rectangle = new SerializableRectangle()
				{
					Position = new SerializablePoint()
					{
						X = X,
						Y = Y
					},
					Size = new SerializableSize()
					{
						W = W,
						H = H
					}
				}
			};
		}
	}
}