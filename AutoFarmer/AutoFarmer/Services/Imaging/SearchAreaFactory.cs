using AutoFarmer.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFarmer.Services.Imaging
{
	public class SearchAreaFactory
	{
		private enum Size
		{
			Width,
			Height
		}

		private enum Position
		{
			X,
			Y
		}

		private enum NamedSearchAreasSizes
		{
			OneQuarter = 0b__001,
			TwoQuarter = 0b__010,
			ThreeQuarter = 0b__011,
			FourQuarter = 0b__100
		}

		private enum NamedSearchAreasPositions
		{
			Zero = 0b__00,
			OneQuarter = 0b__01,
			TwoQuarter = 0b__10
		}

		public static SerializableRectangle FromEnum(NamedSearchArea area)
		{
			SerializableRectangle rec = new SerializableRectangle();

			int qX = Config.Instance.ScreenSize.W / 4;
			int qY = Config.Instance.ScreenSize.H / 4;
			int qW = qX;
			int qH = qY;

			foreach (var p in Enum.GetValues(typeof(NamedSearchAreasPositions)).Cast<NamedSearchAreasPositions>())
			{
				if (Is(area, p, Position.X))
				{
					rec.Position.X = qX * (int)p;
				}
				if (Is(area, p, Position.Y))
				{
					rec.Position.Y = qY * (int)p;
				}
			}

			foreach (var p in Enum.GetValues(typeof(NamedSearchAreasSizes)).Cast<NamedSearchAreasSizes>())
			{
				if (Is(area, p, Size.Width))
				{
					rec.Size.W = qW * (int)p;
				}
				if (Is(area, p, Size.Height))
				{
					rec.Size.H = qH * (int)p;
				}
			}

			return rec;
		}

		public static List<SerializableRectangle> FromEnums(params NamedSearchArea[] searchAreas)
		{
			return searchAreas.Select(a => FromEnum(a)).ToList();
		}

		private static bool Is(NamedSearchArea area, NamedSearchAreasSizes size, Size dimension)
		{
			int normalizedSize = dimension == Size.Width ? (int)size << 3 : (int)size;

			return ((int)area & normalizedSize) == normalizedSize;
		}

		private static bool Is(NamedSearchArea area, NamedSearchAreasPositions position, Position dimension)
		{
			int normalizedPosition = dimension == Position.X ? (int)position << 8 : (int)position << 6;

			return ((int)area & normalizedPosition) == normalizedPosition;
		}
	}
}
