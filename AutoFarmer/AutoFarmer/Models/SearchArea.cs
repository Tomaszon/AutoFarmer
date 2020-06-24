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
				X = Is(searchArea, 0b1000) ? 0 : Config.Instance.ScreenSize.W / 2 + w / 2 - w,
				Y = Is(searchArea, 0b0100) ? 0 : Config.Instance.ScreenSize.H / 2 + h / 2 - h,
				W = Is(searchArea, 0b0010) ? Config.Instance.ScreenSize.W : Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Is(searchArea, 0b0001) ? Config.Instance.ScreenSize.H : Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}

		public static List<SerializableRectangle> FromEnums(int w, int h, params NamedSearchArea[] searchAreas)
		{
			return searchAreas.Select(a => FromEnum(w, h, a)).ToList();
		}

		private static bool Is(NamedSearchArea searchArea, int flag)
		{
			return ((int)searchArea & flag) == flag;
		}
	}
}
