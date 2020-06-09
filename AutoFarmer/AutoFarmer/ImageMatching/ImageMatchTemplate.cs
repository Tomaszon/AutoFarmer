using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AutoFarmer
{
	public class ImageMatchTemplate
	{
		public string Name { get; set; }

		public Bitmap Bitmap { get; private set; }

		public Dictionary<string, SearchRectangle> SearchRectangles { get; set; }

		public void LoadBitmap(string fileName)
		{
			Bitmap = (Bitmap)Image.FromFile(fileName);
		}

		public static ImageMatchTemplate FromJsonFile(string path, string resourcesDirectory)
		{
			var template = JsonConvert.DeserializeObject<ImageMatchTemplate>(File.ReadAllText(path));
			template.Name = Path.GetFileNameWithoutExtension(path);
			template.LoadBitmap(Path.Combine(resourcesDirectory, Path.GetFileName(path)));

			return template;
		}
	}
}
