using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarmer.Models
{
	public class SearchArea
	{
		public static SerializableRectangle Left(int w)
		{
			return new SerializableRectangle()
			{
				X = 0,
				Y = 0,
				W = Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Config.Instance.ScreenSize.H
			};
		}

		public static SerializableRectangle Right(int w)
		{
			return new SerializableRectangle()
			{
				X = Config.Instance.ScreenSize.W / 2 + w / 2 - w,
				Y = 0,
				W = Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Config.Instance.ScreenSize.H
			};
		}

		public static SerializableRectangle Upper(int h)
		{
			return new SerializableRectangle()
			{
				X = 0,
				Y = 0,
				W = Config.Instance.ScreenSize.W,
				H = Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}

		public static SerializableRectangle Lower(int h)
		{
			return new SerializableRectangle()
			{
				X = 0,
				Y = Config.Instance.ScreenSize.H / 2 + h / 2 - h,
				W = Config.Instance.ScreenSize.W,
				H = Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}

		public static SerializableRectangle UpperLeft(int w, int h)
		{
			return new SerializableRectangle()
			{
				X = 0,
				Y = 0,
				W = Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}

		public static SerializableRectangle UpperRight(int w, int h)
		{
			return new SerializableRectangle()
			{
				X = Config.Instance.ScreenSize.W / 2 + w / 2 - w,
				Y = 0,
				W = Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}

		public static SerializableRectangle LowerLeft(int w, int h)
		{
			return new SerializableRectangle()
			{
				X = 0,
				Y = Config.Instance.ScreenSize.H / 2 + h / 2 - h,
				W = Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}

		public static SerializableRectangle LowerRight(int w, int h)
		{
			return new SerializableRectangle()
			{
				X = Config.Instance.ScreenSize.W / 2 + w / 2 - w,
				Y = Config.Instance.ScreenSize.H / 2 + h / 2 - h,
				W = Config.Instance.ScreenSize.W / 2 + w / 2,
				H = Config.Instance.ScreenSize.H / 2 + h / 2
			};
		}
	}
}
