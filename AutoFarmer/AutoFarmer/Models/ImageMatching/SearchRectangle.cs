using AutoFarmer.Models.Common;
using AutoFarmer.Services.Imaging;
using System.Collections.Generic;

namespace AutoFarmer.Models.ImageMatching
{
	public class SearchRectangle : SearchRectangleBase
	{
		public SerializableRectangle Rectangle { get; set; }

		public List<SerializableRectangle> SearchAreas { get; set; }

		public SerializableSize RelativeClickPoint
		{
			get
			{
				return ClickPoint == null ? new SerializableSize() { W = Rectangle.Size.W / 2, H = Rectangle.Size.H / 2 } : new SerializableSize() { W = ClickPoint.X - Rectangle.Position.X, H = ClickPoint.Y - Rectangle.Position.Y };
			}
		}

		public SearchRectangle(SerializablePoint? clickPoint, List<SerializableRectangle>? searchAreas, List<NamedSearchArea>? namedSearchAreas, int x, int y, int w, int h)
		{
			ClickPoint = clickPoint;
			Rectangle = new SerializableRectangle()
			{
				Position = new SerializablePoint()
				{
					X = x,
					Y = y
				},
				Size = new SerializableSize()
				{
					W = w,
					H = h
				}
			};

			SearchAreas ??= new List<SerializableRectangle>();

			if (searchAreas is { })
			{
				SearchAreas.AddRange(searchAreas);
			}

			if (namedSearchAreas is { })
			{
				SearchAreas.AddRange(SearchAreaFactory.FromEnums(namedSearchAreas));
			}

			if (SearchAreas.Count == 0)
			{
				SearchAreas.Add(SearchAreaFactory.FromEnum(NamedSearchArea.Full));
			}
		}
	}
}