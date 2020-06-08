using System.Collections.Generic;
using System.Drawing;

namespace AutoFarmer
{
	public class Template
	{
		public Bitmap Bitmap { get; private set; }

		public Dictionary<string, SearchRectangle> SearchRectangles { get; set; }

		public void LoadBitmap(string fileName)
		{
			Bitmap = (Bitmap)System.Drawing.Image.FromFile(fileName);
		}
	}
}
