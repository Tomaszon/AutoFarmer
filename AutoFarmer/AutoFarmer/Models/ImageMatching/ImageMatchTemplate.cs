using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchTemplate
	{
		public string Name { get; set; }

		public Bitmap Bitmap { get; set; }

		public Dictionary<string, SearchRectangle> SearchRectangles { get; set; }

		public static ImageMatchTemplate FromJsonFile(string path, string resourcesDirectory)
		{
			var template = JsonConvert.DeserializeObject<ImageMatchTemplate>(File.ReadAllText(path));
			template.Name = Path.GetFileNameWithoutExtension(path);

			foreach (var searchRectangle in template.SearchRectangles)
			{
				searchRectangle.Value.Init();
			}

			var resourceName = Directory.GetFiles(resourcesDirectory).First(e => Path.GetFileNameWithoutExtension(e) == template.Name);
			template.Bitmap = (Bitmap)Image.FromFile(resourceName);

			return template;
		}
	}
}
