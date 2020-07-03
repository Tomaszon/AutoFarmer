using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchTemplateOptions
	{
		public string Name { get; set; }

		public Dictionary<string, SearchRectangleOptions> SearchRectangles { get; set; }

		public static ImageMatchTemplateOptions FromJsonFile(string path)
		{
			var templateOptions = JsonConvert.DeserializeObject<ImageMatchTemplateOptions>(File.ReadAllText(path));
			templateOptions.Name = Path.GetFileNameWithoutExtension(path);

			return templateOptions;
		}

		public ImageMatchTemplate ToImageMatchTemplate()
		{
			var template = new ImageMatchTemplate()
			{
				Name = Name,
				SearchRectangles = SearchRectangles.ToDictionary(t => t.Key, v => v.Value.ToSearchRectangle())
			};

			template.Init();

			return template;
		}
	}
}
