using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarmer.Models
{
	public class SearchAreas
	{
		public static SerializableRectangle FromEnum(int w, int h, NamedSearchArea searchArea)
		{
			return new SerializableRectangle()
			{
				X = Is(searchArea, NamedSearchArea.Left) ? 0 : Config.Instance.ScreenSize.W / 2 + w / 2 - w,
				Y = Is(searchArea, NamedSearchArea.Upper) ? 0 : Config.Instance.ScreenSize.H / 2 + h / 2 - h,
				W = Is(searchArea, NamedSearchArea.Upper, NamedSearchArea.Lower) ? Config.Instance.ScreenSize.W : Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Is(searchArea, NamedSearchArea.Left, NamedSearchArea.Right) ? Config.Instance.ScreenSize.H : Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}

		public static List<SerializableRectangle> FromEnums(int w, int h, params NamedSearchArea[] searchAreas)
		{
			var result = new List<SerializableRectangle>();

			Array.ForEach(searchAreas, a => result.Add(FromEnum(w, h, a)));

			return result;
		}

		private static bool Is(NamedSearchArea na1, NamedSearchArea na2)
		{
			return (int)na1 % (int)na2 == 0;
		}

		private static bool Is(NamedSearchArea namedSearchArea, params NamedSearchArea[] namedSearchAreas)
		{
			return namedSearchAreas?.Contains(namedSearchArea) ?? false;
		}
	}
}
