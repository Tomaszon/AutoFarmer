using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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

			var resourceName = Directory.GetFiles(resourcesDirectory).First(e => Path.GetFileNameWithoutExtension(e) == template.Name);

			template.LoadBitmap(resourceName);

			return template;
		}
	}
}
