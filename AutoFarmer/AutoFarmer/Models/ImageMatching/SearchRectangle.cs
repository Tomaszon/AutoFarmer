using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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

		public AutoSearchAreaMode AutoSearchAreaMode { get; set; } = AutoSearchAreaMode.LeftRight;

		public SerializableSize RelativeClickPoint
		{
			get
			{
				return ClickPoint == null ? new SerializableSize() { W = W / 2, H = H / 2 } : new SerializableSize() { W = ClickPoint.X - X, H = ClickPoint.Y - Y };
			}
		}

		public SearchRectangle Scale(double scale)
		{
			return new SearchRectangle()
			{
				X = (int)(X * scale),
				Y = (int)(Y * scale),
				W = (int)(W * scale),
				H = (int)(H * scale),
				AutoSearchAreaMode = AutoSearchAreaMode,
				ClickPoint = ClickPoint?.Scale(scale),
				NamedSearchAreas = NamedSearchAreas,
				SearchAreas = SearchAreas.Select(a => a.Scale(scale)).ToList()
			};
		}

		public void Init()
		{
			if (SearchAreas.Count != 0) return;

			if (NamedSearchAreas != null)
			{
				Array.ForEach(NamedSearchAreas, a => SearchAreas.Add(Models.SearchAreas.FromEnum(W, H, a)));
			}
			else
			{
				switch (AutoSearchAreaMode)
				{
					case AutoSearchAreaMode.Full:
					{
						SearchAreas.AddRange(Models.SearchAreas.FromEnums(W, H, NamedSearchArea.Full));
						break;
					}
					case AutoSearchAreaMode.LeftRight:
					{
						SearchAreas.AddRange(Models.SearchAreas.FromEnums(W, H, NamedSearchArea.Left, NamedSearchArea.Right));
						break;
					}
					case AutoSearchAreaMode.UpperLower:
					{
						SearchAreas.AddRange(Models.SearchAreas.FromEnums(W, H, NamedSearchArea.Upper, NamedSearchArea.Lower));

						break;
					}
					case AutoSearchAreaMode.Quarter:
					{
						SearchAreas.AddRange(Models.SearchAreas.FromEnums(W, H, NamedSearchArea.UpperLeft,
							NamedSearchArea.UpperRight, NamedSearchArea.LowerLeft, NamedSearchArea.LowerRight));

						break;
					}
				}
			}
		}

		public static implicit operator Rectangle(SearchRectangle rec)
		{
			return new Rectangle(rec.X, rec.Y, rec.W, rec.H);
		}
	}
}