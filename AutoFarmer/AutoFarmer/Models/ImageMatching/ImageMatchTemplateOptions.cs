using AutoFarmer.Models.Graph;
using AutoFarmer.Services.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AutoFarmer.Models.Graph.IOptions;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchTemplateOptions : IOptions
	{
		public string Name { get; set; }

		public Dictionary<string, SearchRectangleOptions> SearchRectangles { get; set; }

		public static ImageMatchTemplateOptions FromJsonFile(string path)
		{
			return FromJsonFileWrapper(() =>
			{
				using var log = Logger.LogBlock();

				var templateOptions = JsonConvert.DeserializeObject<ImageMatchTemplateOptions>(File.ReadAllText(path));
				templateOptions.Name = Path.GetFileNameWithoutExtension(path);

				return templateOptions;
			});
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
