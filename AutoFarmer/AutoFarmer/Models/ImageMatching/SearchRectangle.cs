using AutoFarmer.Models.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AutoFarmer.Models.ImageMatching
{
	public class SearchRectangle
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int W { get; set; }

		public int H { get; set; }

		public SerializablePoint ClickPoint { get; set; }

		public List<SerializableRectangle> SearchAreas { get; set; } = new List<SerializableRectangle>();

		public NamedSearchArea[] NamedSearchAreas { get; set; }

		public SerializableSize RelativeClickPoint
		{
			get
			{
				return ClickPoint == null ? new SerializableSize() { W = W / 2, H = H / 2 } : new SerializableSize() { W = ClickPoint.X - X, H = ClickPoint.Y - Y };
			}
		}

		public void Init()
		{
			if (SearchAreas.Count != 0)
			{
				if (IsOverlaping(out var intersectingRectangles))
				{
					throw new AutoFarmerException($"Custom search areas overlapping: {JsonConvert.SerializeObject(intersectingRectangles, Formatting.Indented)}");
				}
			}
			else if (NamedSearchAreas != null)
			{
				SearchAreas.AddRange(SearchAreaFactory.FromEnums(W, H, NamedSearchAreas));
			}
		}

		private bool IsOverlaping(out List<Tuple<SerializableRectangle, List<SerializableRectangle>>> intersectingRectangles)
		{
			intersectingRectangles = new List<Tuple<SerializableRectangle, List<SerializableRectangle>>>();

			foreach (var a1 in SearchAreas)
			{
				foreach (var a2 in SearchAreas)
				{
					if (!a1.Equals(a2))
					{
						if (intersectingRectangles.FirstOrDefault(e => e.Item1.Equals(a1)) is var a && a != null)
						{
							a.Item2.Add(a2);
						}
						else
						{
							intersectingRectangles.Add(new Tuple<SerializableRectangle, List<SerializableRectangle>>(a1, new List<SerializableRectangle>() { a2 }));
						}
					}
				}
			}

			return intersectingRectangles.Count > 0;
		}

		public static explicit operator Rectangle(SearchRectangle rec)
		{
			return new Rectangle(rec.X, rec.Y, rec.W, rec.H);
		}
	}
}