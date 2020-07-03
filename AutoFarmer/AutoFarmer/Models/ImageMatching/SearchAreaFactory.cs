using AutoFarmer.Models.Common;
using System.Collections.Generic;
using System.Linq;

namespace AutoFarmer.Models.ImageMatching
{
	public class SearchAreaFactory
	{
		public static SerializableRectangle FromEnum(NamedSearchArea searchArea)
		{
			if (searchArea == NamedSearchArea.MiddleHalfWidth)
			{
				return new SerializableRectangle()
				{
					Position = new SerializablePoint()
					{
						X = Config.Instance.ScreenSize.W / 4,
						Y = 0
					},
					Size = new SerializableSize()
					{
						W = Config.Instance.ScreenSize.W / 2,
						H = Config.Instance.ScreenSize.H
					}
				};
			}
			//TODO
			else if (searchArea == NamedSearchArea.MiddleHalfHeight)
			{
				return null;
			}
			else if (searchArea == NamedSearchArea.Middle)
			{
				return null;
			}
			else
			{
				return new SerializableRectangle()
				{
					Position = new SerializablePoint()
					{
						X = Is(searchArea, 0b1000) ? 0 : Config.Instance.ScreenSize.W / 2,
						Y = Is(searchArea, 0b0100) ? 0 : Config.Instance.ScreenSize.H / 2
					},
					Size = new SerializableSize()
					{
						W = Is(searchArea, 0b0010) ? Config.Instance.ScreenSize.W : Config.Instance.ScreenSize.W / 2,
						H = Is(searchArea, 0b0001) ? Config.Instance.ScreenSize.H : Config.Instance.ScreenSize.H / 2
					}
				};
			}
		}

		public static List<SerializableRectangle> FromEnums(params NamedSearchArea[] searchAreas)
		{
			return searchAreas.Select(a => FromEnum(a)).ToList();
		}

		private static bool Is(NamedSearchArea searchArea, int flag)
		{
			return ((int)searchArea & flag) == flag;
		}
	}
}
